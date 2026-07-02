from datetime import date, datetime, timezone
from decimal import Decimal
import json

from fastapi import Depends, FastAPI, File, Form, HTTPException, Query, UploadFile
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy import func, or_, select, text
from sqlalchemy.orm import Session

from app.config import settings
from app.db import get_db
from app.deps import current_user, require_admin
from app.import_ai import DEFAULT_GEMINI_MODEL, DEFAULT_OPENAI_MODEL, manual_transform_transactions, spreadsheet_rows, transform_transactions, table_excerpt
from app.models import Account, AccountLedgerEntry, AccountTransfer, AgencyLedgerEntry, AgencyLink, AgencyLinkStatus, AgencyTransferRule, AgencyTransferRuleStatus, AuditArea, AuditLog, CashCount, Company, InterAgencySettlement, InterAgencyTransfer, InterAgencyTransferStatus, Service, ServiceFeeRule, ServiceTransaction, UnpaidItem, UnpaidMovement, User, UserRole
from app.schemas import (
    AccountCreate,
    AccountBalanceUpdate,
    AccountOut,
    AccountsScreenSettingsIn,
    AgencyLedgerEntryIn,
    AgencyLedgerEntryOut,
    AgencyCreate,
    AgencyLinkCreate,
    AgencyLinkOut,
    AgencyTransferRuleCreate,
    AgencyTransferRuleOut,
    CashCountIn,
    CompanyOut,
    CredentialsUpdate,
    DashboardSummary,
    FeeRuleCreate,
    LoginIn,
    ProfileUpdate,
    RegisterIn,
    InterAgencyTransferCreate,
    InterAgencyTransferOut,
    InterAgencySettlementCreate,
    InterAgencySettlementOut,
    SalafEntryIn,
    ServiceCreate,
    ServiceOut,
    ServiceTransactionCreate,
    ServiceTransactionUpdate,
    ServiceUpdate,
    Token,
    TransferCreate,
    UnpaidItemIn,
    UnpaidItemOut,
    UnpaidMovementIn,
    UserCreate,
    UserUpdate,
    UserOut,
)
from app.security import create_access_token, hash_password, verify_password
from app.services import accept_inter_agency_transfer, account_display_balance, apply_balance_delta, audit as write_audit, create_salaf, create_service_transaction, create_transfer, dashboard_summary, reset_account_opening_balance, reverse_service_transaction, save_cash_count, sync_account_balance, update_service_transaction

app = FastAPI(title="RDET Web API")

app.add_middleware(
    CORSMiddleware,
    allow_origins=settings.cors_origin_list,
    allow_origin_regex=settings.cors_origin_regex,
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.on_event("startup")
def ensure_multi_agency_schema() -> None:
    from app.db import SessionLocal

    with SessionLocal() as db:
      db.execute(text("ALTER TABLE users ADD COLUMN IF NOT EXISTS email TEXT"))
      db.execute(text("ALTER TABLE users ADD COLUMN IF NOT EXISTS phone TEXT"))
      db.execute(text("ALTER TABLE users ADD COLUMN IF NOT EXISTS permissions JSONB"))
      db.execute(text("ALTER TABLE accounts ADD COLUMN IF NOT EXISTS previous_balance NUMERIC(14,2)"))
      db.execute(text("ALTER TABLE accounts ADD COLUMN IF NOT EXISTS debit_total NUMERIC(14,2) NOT NULL DEFAULT 0"))
      db.execute(text("ALTER TABLE accounts ADD COLUMN IF NOT EXISTS credit_total NUMERIC(14,2) NOT NULL DEFAULT 0"))
      db.execute(text("ALTER TABLE accounts ADD COLUMN IF NOT EXISTS normal_balance_side TEXT NOT NULL DEFAULT 'debit'"))
      db.execute(text("""
          DO $$
          BEGIN
              ALTER TABLE accounts ADD CONSTRAINT accounts_normal_balance_side_check CHECK (normal_balance_side IN ('debit', 'credit')) NOT VALID;
          EXCEPTION WHEN duplicate_object THEN NULL;
          END $$;
      """))
      db.execute(text("""
          UPDATE accounts
          SET debit_total = CASE WHEN COALESCE(balance, 0) >= 0 THEN COALESCE(balance, 0) ELSE 0 END,
              credit_total = CASE WHEN COALESCE(balance, 0) < 0 THEN ABS(COALESCE(balance, 0)) ELSE 0 END
          WHERE COALESCE(debit_total, 0) = 0
            AND COALESCE(credit_total, 0) = 0
            AND COALESCE(balance, 0) <> 0
      """))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS account_ledger_entries (
              id BIGSERIAL PRIMARY KEY,
              account_id BIGINT NOT NULL REFERENCES accounts(id),
              account_transfer_id BIGINT REFERENCES account_transfers(id),
              side TEXT NOT NULL CHECK (side IN ('debit', 'credit')),
              amount NUMERIC(14,2) NOT NULL CHECK (amount >= 0),
              balance_effect NUMERIC(14,2) NOT NULL,
              balance_after NUMERIC(14,2) NOT NULL DEFAULT 0,
              description TEXT,
              occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              created_by BIGINT REFERENCES users(id)
          )
      """))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_account_ledger_account ON account_ledger_entries(account_id, occurred_at DESC, id DESC)"))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_account_ledger_transfer ON account_ledger_entries(account_transfer_id)"))
      db.execute(text("""
          INSERT INTO account_ledger_entries (
              account_id,
              account_transfer_id,
              side,
              amount,
              balance_effect,
              balance_after,
              description,
              occurred_at,
              created_by
          )
          SELECT
              entry.account_id,
              entry.transfer_id,
              CASE
                  WHEN entry.normal_balance_side = 'credit' AND entry.balance_effect > 0 THEN 'credit'
                  WHEN entry.normal_balance_side = 'credit' THEN 'debit'
                  WHEN entry.balance_effect > 0 THEN 'debit'
                  ELSE 'credit'
              END,
              ABS(entry.balance_effect),
              entry.balance_effect,
              0,
              entry.description,
              entry.occurred_at,
              entry.created_by
          FROM (
              SELECT
                  transfers.id AS transfer_id,
                  transfers.from_account_id AS account_id,
                  accounts.normal_balance_side,
                  -transfers.amount AS balance_effect,
                  transfers.description,
                  transfers.occurred_at,
                  transfers.created_by
              FROM account_transfers transfers
              JOIN accounts ON accounts.id = transfers.from_account_id
              WHERE transfers.from_account_id IS NOT NULL
              UNION ALL
              SELECT
                  transfers.id AS transfer_id,
                  transfers.to_account_id AS account_id,
                  accounts.normal_balance_side,
                  transfers.amount AS balance_effect,
                  transfers.description,
                  transfers.occurred_at,
                  transfers.created_by
              FROM account_transfers transfers
              JOIN accounts ON accounts.id = transfers.to_account_id
              WHERE transfers.to_account_id IS NOT NULL
          ) entry
          WHERE NOT EXISTS (
              SELECT 1
              FROM account_ledger_entries existing
              WHERE existing.account_transfer_id = entry.transfer_id
                AND existing.account_id = entry.account_id
                AND existing.balance_effect = entry.balance_effect
              )
      """))
      db.execute(text("""
          WITH source_rows AS (
              SELECT
                  ledger.id AS ledger_id,
                  ledger.account_id,
                  ledger.amount,
                  accounts.normal_balance_side
              FROM account_ledger_entries ledger
              JOIN inter_agency_transfers transfers
                ON transfers.account_transfer_id = ledger.account_transfer_id
              JOIN accounts
                ON accounts.id = ledger.account_id
              WHERE transfers.status = 'accepted'
                AND ledger.account_id = transfers.source_account_id
                AND ledger.balance_effect < 0
          ),
          account_deltas AS (
              SELECT
                  account_id,
                  normal_balance_side,
                  SUM(amount) AS amount
              FROM source_rows
              GROUP BY account_id, normal_balance_side
          )
          UPDATE accounts
          SET debit_total = CASE
                  WHEN account_deltas.normal_balance_side = 'debit' THEN accounts.debit_total + account_deltas.amount
                  ELSE GREATEST(accounts.debit_total - account_deltas.amount, 0)
              END,
              credit_total = CASE
                  WHEN account_deltas.normal_balance_side = 'credit' THEN accounts.credit_total + account_deltas.amount
                  ELSE GREATEST(accounts.credit_total - account_deltas.amount, 0)
              END
          FROM account_deltas
          WHERE accounts.id = account_deltas.account_id
      """))
      db.execute(text("""
          UPDATE account_ledger_entries ledger
          SET side = CASE
                  WHEN accounts.normal_balance_side = 'credit' THEN 'credit'
                  ELSE 'debit'
              END,
              balance_effect = ledger.amount,
              balance_after = CASE
                  WHEN accounts.normal_balance_side = 'credit' THEN accounts.credit_total - accounts.debit_total
                  ELSE accounts.debit_total - accounts.credit_total
              END
          FROM inter_agency_transfers transfers, accounts
          WHERE transfers.account_transfer_id = ledger.account_transfer_id
            AND accounts.id = ledger.account_id
            AND transfers.status = 'accepted'
            AND ledger.account_id = transfers.source_account_id
            AND ledger.balance_effect < 0
      """))
      db.execute(text("ALTER TABLE account_transfers ADD COLUMN IF NOT EXISTS contributions JSONB"))
      db.execute(text("ALTER TABLE services ADD COLUMN IF NOT EXISTS company_id BIGINT REFERENCES companies(id)"))
      db.execute(text("ALTER TABLE services ADD COLUMN IF NOT EXISTS image_url TEXT"))
      db.execute(text("ALTER TABLE services ADD COLUMN IF NOT EXISTS routing_config JSONB"))
      db.execute(text("ALTER TABLE services ALTER COLUMN primary_account_id DROP NOT NULL"))
      db.execute(text("ALTER TABLE services ALTER COLUMN secondary_account_id DROP NOT NULL"))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS unpaid_movements (
              id BIGSERIAL PRIMARY KEY,
              person_name TEXT NOT NULL,
              direction TEXT NOT NULL,
              amount NUMERIC(14,2) NOT NULL,
              description TEXT,
              occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              created_by BIGINT REFERENCES users(id)
          )
      """))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS agency_settings (
              company_id BIGINT NOT NULL REFERENCES companies(id) ON DELETE CASCADE,
              key TEXT NOT NULL,
              value JSONB NOT NULL DEFAULT '{}',
              updated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              PRIMARY KEY (company_id, key)
          )
      """))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS agency_ledger_entries (
              id BIGSERIAL PRIMARY KEY,
              kind TEXT NOT NULL,
              category TEXT NOT NULL,
              amount NUMERIC(14,2) NOT NULL,
              description TEXT,
              occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              created_by BIGINT REFERENCES users(id)
          )
      """))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS user_agencies (
              user_id BIGINT NOT NULL REFERENCES users(id) ON DELETE CASCADE,
              company_id BIGINT NOT NULL REFERENCES companies(id) ON DELETE CASCADE,
              role user_role NOT NULL DEFAULT 'Admin',
              created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              PRIMARY KEY (user_id, company_id)
          )
      """))
      db.execute(text("DO $$ BEGIN CREATE TYPE agency_link_status AS ENUM ('pending', 'active', 'rejected', 'disabled'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;"))
      db.execute(text("DO $$ BEGIN CREATE TYPE agency_transfer_rule_status AS ENUM ('pending', 'active', 'rejected', 'disabled'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;"))
      db.execute(text("DO $$ BEGIN CREATE TYPE inter_agency_transfer_status AS ENUM ('pending_receiver', 'accepted', 'cancelled', 'rejected'); EXCEPTION WHEN duplicate_object THEN NULL; END $$;"))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS agency_links (
              id BIGSERIAL PRIMARY KEY,
              agency_a_id BIGINT NOT NULL REFERENCES companies(id),
              agency_b_id BIGINT NOT NULL REFERENCES companies(id),
              status agency_link_status NOT NULL DEFAULT 'pending',
              requested_agency_id BIGINT REFERENCES companies(id),
              target_agency_id BIGINT REFERENCES companies(id),
              requested_by_user_id BIGINT REFERENCES users(id),
              accepted_by_user_id BIGINT REFERENCES users(id),
              created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              accepted_at TIMESTAMPTZ,
              disabled_at TIMESTAMPTZ,
              CHECK (agency_a_id <> agency_b_id),
              UNIQUE (agency_a_id, agency_b_id)
          )
      """))
      db.execute(text("ALTER TABLE agency_links ADD COLUMN IF NOT EXISTS requested_agency_id BIGINT REFERENCES companies(id)"))
      db.execute(text("ALTER TABLE agency_links ADD COLUMN IF NOT EXISTS target_agency_id BIGINT REFERENCES companies(id)"))
      db.execute(text("""
          UPDATE agency_links links
          SET requested_agency_id = users.company_id
          FROM users
          WHERE links.requested_agency_id IS NULL
            AND links.requested_by_user_id = users.id
            AND users.company_id IN (links.agency_a_id, links.agency_b_id)
            AND links.status <> 'pending'
      """))
      db.execute(text("""
          UPDATE agency_links
          SET target_agency_id = CASE
              WHEN requested_agency_id = agency_a_id THEN agency_b_id
              WHEN requested_agency_id = agency_b_id THEN agency_a_id
              ELSE target_agency_id
          END
          WHERE target_agency_id IS NULL
            AND status <> 'pending'
      """))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS agency_transfer_rules (
              id BIGSERIAL PRIMARY KEY,
              agency_link_id BIGINT NOT NULL REFERENCES agency_links(id),
              source_agency_id BIGINT NOT NULL REFERENCES companies(id),
              source_account_id BIGINT NOT NULL REFERENCES accounts(id),
              destination_agency_id BIGINT NOT NULL REFERENCES companies(id),
              destination_account_id BIGINT NOT NULL REFERENCES accounts(id),
              name TEXT NOT NULL,
              description TEXT,
              status agency_transfer_rule_status NOT NULL DEFAULT 'pending',
              active BOOLEAN NOT NULL DEFAULT TRUE,
              created_by_user_id BIGINT REFERENCES users(id),
              accepted_by_user_id BIGINT REFERENCES users(id),
              created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              accepted_at TIMESTAMPTZ,
              disabled_at TIMESTAMPTZ,
              CHECK (source_agency_id <> destination_agency_id OR source_account_id <> destination_account_id)
          )
      """))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS inter_agency_transfers (
              id BIGSERIAL PRIMARY KEY,
              transfer_rule_id BIGINT NOT NULL REFERENCES agency_transfer_rules(id),
              source_agency_id BIGINT NOT NULL REFERENCES companies(id),
              source_account_id BIGINT NOT NULL REFERENCES accounts(id),
              destination_agency_id BIGINT NOT NULL REFERENCES companies(id),
              destination_account_id BIGINT NOT NULL REFERENCES accounts(id),
              amount NUMERIC(14,2) NOT NULL CHECK (amount > 0),
              note TEXT,
              status inter_agency_transfer_status NOT NULL DEFAULT 'pending_receiver',
              created_by_user_id BIGINT REFERENCES users(id),
              receiver_decision_by_user_id BIGINT REFERENCES users(id),
              account_transfer_id BIGINT REFERENCES account_transfers(id),
              created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              decided_at TIMESTAMPTZ
          )
      """))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_agency_links_agencies ON agency_links(agency_a_id, agency_b_id)"))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_agency_transfer_rules_agencies ON agency_transfer_rules(source_agency_id, destination_agency_id)"))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_inter_agency_transfers_status ON inter_agency_transfers(status)"))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_inter_agency_transfers_agencies ON inter_agency_transfers(source_agency_id, destination_agency_id)"))
      db.execute(text("""
          CREATE TABLE IF NOT EXISTS inter_agency_settlements (
              id BIGSERIAL PRIMARY KEY,
              inter_agency_transfer_id BIGINT NOT NULL REFERENCES inter_agency_transfers(id),
              payer_agency_id BIGINT NOT NULL REFERENCES companies(id),
              payer_account_id BIGINT NOT NULL REFERENCES accounts(id),
              receiver_agency_id BIGINT NOT NULL REFERENCES companies(id),
              receiver_account_id BIGINT NOT NULL REFERENCES accounts(id),
              debt_account_id BIGINT NOT NULL REFERENCES accounts(id),
              amount NUMERIC(14,2) NOT NULL CHECK (amount > 0),
              note TEXT,
              status TEXT NOT NULL DEFAULT 'pending',
              account_transfer_id BIGINT REFERENCES account_transfers(id),
              created_by_user_id BIGINT REFERENCES users(id),
              accepted_by_user_id BIGINT REFERENCES users(id),
              created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
              accepted_at TIMESTAMPTZ
          )
      """))
      db.execute(text("ALTER TABLE inter_agency_settlements ADD COLUMN IF NOT EXISTS status TEXT NOT NULL DEFAULT 'pending'"))
      db.execute(text("ALTER TABLE inter_agency_settlements ADD COLUMN IF NOT EXISTS accepted_by_user_id BIGINT REFERENCES users(id)"))
      db.execute(text("ALTER TABLE inter_agency_settlements ADD COLUMN IF NOT EXISTS accepted_at TIMESTAMPTZ"))
      db.execute(text("UPDATE inter_agency_settlements SET status = 'accepted', accepted_at = created_at WHERE account_transfer_id IS NOT NULL AND status = 'pending'"))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_inter_agency_settlements_transfer ON inter_agency_settlements(inter_agency_transfer_id)"))
      db.execute(text("CREATE INDEX IF NOT EXISTS idx_inter_agency_settlements_agencies ON inter_agency_settlements(payer_agency_id, receiver_agency_id)"))
      db.execute(text("""
          INSERT INTO user_agencies (user_id, company_id, role)
          SELECT id, company_id, role FROM users
          WHERE company_id IS NOT NULL
          ON CONFLICT DO NOTHING
      """))
      db.execute(text("SELECT setval(pg_get_serial_sequence('companies', 'id'), COALESCE((SELECT MAX(id) FROM companies), 1), true)"))
      db.execute(text("SELECT setval(pg_get_serial_sequence('users', 'id'), COALESCE((SELECT MAX(id) FROM users), 1), true)"))
      db.execute(text("SELECT setval(pg_get_serial_sequence('accounts', 'id'), COALESCE((SELECT MAX(id) FROM accounts), 1), true)"))
      db.commit()


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.post("/auth/login", response_model=Token)
def login(payload: LoginIn, db: Session = Depends(get_db)) -> Token:
    user = db.scalar(select(User).where(User.username == payload.username))
    if not user or not verify_password(db, payload.password, user.password_hash):
        raise HTTPException(status_code=401, detail="Invalid username or password")
    return Token(access_token=create_access_token(user))


@app.post("/auth/register", response_model=Token)
def register(payload: RegisterIn, db: Session = Depends(get_db)) -> Token:
    agency_name = payload.agency_name.strip()
    username = payload.username.strip()
    first_name = payload.first_name.strip()
    last_name = payload.last_name.strip()

    if db.scalar(select(User).where(User.username == username)):
        raise HTTPException(status_code=409, detail="Username already exists")
    if db.scalar(select(Company).where(Company.name == agency_name)):
        raise HTTPException(status_code=409, detail="Agency name already exists")

    company = Company(name=agency_name, description="Agency created from self-service registration", admin_local=f"{first_name} {last_name}")
    db.add(company)
    db.flush()

    user = User(
        company_id=company.id,
        username=username,
        password_hash=hash_password(db, payload.password),
        first_name=first_name,
        last_name=last_name,
        role=UserRole.admin,
        active=True,
    )
    db.add(user)
    db.flush()
    db.execute(text("INSERT INTO user_agencies (user_id, company_id, role) VALUES (:user_id, :company_id, 'Admin') ON CONFLICT DO NOTHING"), {"user_id": user.id, "company_id": company.id})
    db.commit()
    db.refresh(user)
    return Token(access_token=create_access_token(user))


@app.get("/me", response_model=UserOut)
def me(user: User = Depends(current_user)) -> User:
    return user


@app.get("/agencies", response_model=list[CompanyOut])
def list_agencies(user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[Company]:
    agency_ids = [row[0] for row in db.execute(text("SELECT company_id FROM user_agencies WHERE user_id = :user_id"), {"user_id": user.id})]
    if not agency_ids:
        return []
    return list(db.scalars(select(Company).where(Company.id.in_(agency_ids)).order_by(Company.name)))


@app.post("/agencies", response_model=CompanyOut)
def create_agency(payload: AgencyCreate, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> Company:
    name = payload.name.strip()
    if db.scalar(select(Company).where(Company.name == name)):
        raise HTTPException(status_code=409, detail="Agency name already exists")
    company = Company(name=name, description="Agency created from AgencyOS", admin_local=f"{user.first_name} {user.last_name}")
    db.add(company)
    db.flush()
    db.execute(text("INSERT INTO user_agencies (user_id, company_id, role) VALUES (:user_id, :company_id, 'Admin') ON CONFLICT DO NOTHING"), {"user_id": user.id, "company_id": company.id})
    user.company_id = company.id
    db.commit()
    db.refresh(company)
    return company


@app.post("/me/agency/{agency_id}", response_model=UserOut)
def switch_agency(agency_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> User:
    allowed = db.execute(text("SELECT 1 FROM user_agencies WHERE user_id = :user_id AND company_id = :company_id"), {"user_id": user.id, "company_id": agency_id}).scalar()
    if not allowed:
        raise HTTPException(status_code=403, detail="Agency access denied")
    user.company_id = agency_id
    db.commit()
    db.refresh(user)
    return user


@app.patch("/me/profile", response_model=UserOut)
def update_profile(payload: ProfileUpdate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> User:
    user.first_name = payload.first_name.strip()
    user.last_name = payload.last_name.strip()
    user.email = payload.email.strip() if payload.email else None
    user.phone = payload.phone.strip() if payload.phone else None
    user.image_url = payload.image_url.strip() if payload.image_url else None
    db.commit()
    db.refresh(user)
    return user


@app.patch("/me/credentials", response_model=UserOut)
def update_credentials(payload: CredentialsUpdate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> User:
    username = payload.username.strip()
    existing = db.scalar(select(User).where(User.username == username, User.id != user.id))
    if existing:
        raise HTTPException(status_code=409, detail="Username already exists")
    if payload.new_password:
        if not payload.current_password or not verify_password(db, payload.current_password, user.password_hash):
            raise HTTPException(status_code=400, detail="Current password is incorrect")
        user.password_hash = hash_password(db, payload.new_password)
    user.username = username
    db.commit()
    db.refresh(user)
    return user


@app.get("/users", response_model=list[UserOut])
def list_users(user: User = Depends(require_admin), db: Session = Depends(get_db)) -> list[User]:
    query = select(User)
    if user.company_id:
        query = query.where(User.company_id == user.company_id)
    return list(db.scalars(query.order_by(User.id)))


@app.post("/users", response_model=UserOut)
def create_user(payload: UserCreate, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> User:
    username = payload.username.strip()
    if db.scalar(select(User).where(User.username == username)):
        raise HTTPException(status_code=409, detail="Username already exists")
    item = User(
        company_id=user.company_id,
        username=username,
        password_hash=hash_password(db, payload.password),
        first_name=payload.first_name.strip(),
        last_name=payload.last_name.strip(),
        role=payload.role,
        permissions=payload.permissions,
        active=payload.active,
    )
    db.add(item)
    db.flush()
    if user.company_id:
        db.execute(
            text("INSERT INTO user_agencies (user_id, company_id, role) VALUES (:user_id, :company_id, :role) ON CONFLICT (user_id, company_id) DO UPDATE SET role = EXCLUDED.role"),
            {"user_id": item.id, "company_id": user.company_id, "role": payload.role.value},
        )
    db.commit()
    db.refresh(item)
    return item


@app.patch("/users/{user_id}", response_model=UserOut)
def update_user(user_id: int, payload: UserUpdate, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> User:
    item = db.get(User, user_id)
    if not item or (user.company_id and item.company_id != user.company_id):
        raise HTTPException(status_code=404, detail="User not found")
    username = payload.username.strip()
    existing = db.scalar(select(User).where(User.username == username, User.id != user_id))
    if existing:
        raise HTTPException(status_code=409, detail="Username already exists")
    item.first_name = payload.first_name.strip()
    item.last_name = payload.last_name.strip()
    item.username = username
    item.role = payload.role
    item.permissions = payload.permissions
    item.active = payload.active
    if payload.password:
        item.password_hash = hash_password(db, payload.password)
    if item.company_id:
        db.execute(
            text("INSERT INTO user_agencies (user_id, company_id, role) VALUES (:user_id, :company_id, :role) ON CONFLICT (user_id, company_id) DO UPDATE SET role = EXCLUDED.role"),
            {"user_id": item.id, "company_id": item.company_id, "role": payload.role.value},
        )
    db.commit()
    db.refresh(item)
    return item


def accounts_screen_settings(db: Session, company_id: int | None) -> dict:
    if not company_id:
        return {}
    value = db.execute(
        text("SELECT value FROM agency_settings WHERE company_id = :company_id AND key = 'accounts_screen_config'"),
        {"company_id": company_id},
    ).scalar()
    return value if isinstance(value, dict) else {}


def account_action_settings(db: Session, company_id: int | None) -> dict:
    if not company_id:
        return {"rules": []}
    value = db.execute(
        text("SELECT value FROM agency_settings WHERE company_id = :company_id AND key = 'account_action_rules'"),
        {"company_id": company_id},
    ).scalar()
    return value if isinstance(value, dict) else {"rules": []}


def account_visibility_settings(db: Session, company_id: int | None) -> dict[str, dict]:
    settings_value = accounts_screen_settings(db, company_id)
    visibility: dict[str, dict] = {}
    if not isinstance(settings_value, dict):
        return visibility
    for account_id, config in settings_value.items():
        if str(account_id).startswith("__") or not isinstance(config, dict):
            continue
        account_visibility = config.get("visibility")
        if isinstance(account_visibility, dict):
            visibility[str(account_id)] = account_visibility
    return visibility


def hidden_account_ids(db: Session, user: User) -> set[int]:
    if user.role == UserRole.admin:
        return set()
    visibility = account_visibility_settings(db, user.company_id)
    hidden: set[int] = set()
    for account_id, config in visibility.items():
        hidden_roles = [str(item) for item in config.get("hiddenRoles", [])] if isinstance(config.get("hiddenRoles"), list) else []
        hidden_users = [str(item) for item in config.get("hiddenUserIds", [])] if isinstance(config.get("hiddenUserIds"), list) else []
        if user.role.value in hidden_roles or str(user.id) in hidden_users:
            try:
                hidden.add(int(account_id))
            except ValueError:
                pass
    return hidden


def account_is_accessible(account: Account | None, user: User, db: Session) -> bool:
    if not account:
        return False
    if user.company_id and user.company_id not in account.company_ids:
        return False
    if user.role == UserRole.admin:
        return True
    if not account.visible:
        return False
    return account.id not in hidden_account_ids(db, user)


def require_account_access(account: Account | None, user: User, db: Session) -> Account:
    if not account_is_accessible(account, user, db):
        raise HTTPException(status_code=404, detail="Account not found")
    return account


def app_settings(db: Session, company_id: int | None) -> dict:
    if not company_id:
        return {}
    value = db.execute(
        text("SELECT value FROM agency_settings WHERE company_id = :company_id AND key = 'app_settings'"),
        {"company_id": company_id},
    ).scalar()
    return value if isinstance(value, dict) else {}


def has_permission(user: User, db: Session, screen: str, action: str) -> bool:
    if user.role == UserRole.admin:
        return True
    settings_value = app_settings(db, user.company_id)
    role_permissions = settings_value.get("rolePermissions") if isinstance(settings_value, dict) else {}
    role_section = {}
    if isinstance(role_permissions, dict):
        role_section = role_permissions.get(user.role.value, {}).get(screen, {}) if isinstance(role_permissions.get(user.role.value, {}), dict) else {}
    user_permissions = user.permissions if isinstance(user.permissions, dict) else {}
    user_section = user_permissions.get(screen, {}) if isinstance(user_permissions, dict) else {}
    return bool({**role_section, **user_section}.get(action))


def require_permission(user: User, db: Session, screen: str, action: str) -> None:
    if not has_permission(user, db, screen, action):
        raise HTTPException(status_code=403, detail=f"Permission denied: {screen}.{action}")


def require_agency(user: User) -> int:
    if not user.company_id:
        raise HTTPException(status_code=400, detail="Select an active agency first")
    return user.company_id


def require_agency_manager(user: User) -> None:
    if user.role not in {UserRole.admin, UserRole.chef}:
        raise HTTPException(status_code=403, detail="Admin or manager access required")


def account_belongs_to_agency(db: Session, account_id: int, agency_id: int) -> Account:
    account = db.get(Account, account_id)
    if not account or agency_id not in account.company_ids:
        raise HTTPException(status_code=400, detail="Account does not belong to agency")
    return account


def active_link_between(db: Session, agency_a_id: int, agency_b_id: int) -> AgencyLink | None:
    left, right = sorted([agency_a_id, agency_b_id])
    return db.scalar(
        select(AgencyLink).where(
            AgencyLink.agency_a_id == left,
            AgencyLink.agency_b_id == right,
            AgencyLink.status == AgencyLinkStatus.active,
        )
    )


def link_requested_agency_id(link: AgencyLink, db: Session) -> int | None:
    if link.requested_agency_id:
        return link.requested_agency_id
    requester = db.get(User, link.requested_by_user_id) if link.requested_by_user_id else None
    if requester and requester.company_id in {link.agency_a_id, link.agency_b_id}:
        return requester.company_id
    return None


def link_target_agency_id(link: AgencyLink, db: Session) -> int | None:
    if link.target_agency_id:
        return link.target_agency_id
    requested_agency_id = link_requested_agency_id(link, db)
    if requested_agency_id == link.agency_a_id:
        return link.agency_b_id
    if requested_agency_id == link.agency_b_id:
        return link.agency_a_id
    return None


def link_payload(link: AgencyLink) -> dict:
    return {
        "id": link.id,
        "agency_a_id": link.agency_a_id,
        "agency_b_id": link.agency_b_id,
        "agency_a_name": link.agency_a.name if link.agency_a else None,
        "agency_b_name": link.agency_b.name if link.agency_b else None,
        "status": link.status.value,
        "requested_agency_id": link.requested_agency_id,
        "target_agency_id": link.target_agency_id,
        "requested_by_user_id": link.requested_by_user_id,
        "accepted_by_user_id": link.accepted_by_user_id,
        "created_at": link.created_at,
        "accepted_at": link.accepted_at,
        "disabled_at": link.disabled_at,
    }


def rule_payload(rule: AgencyTransferRule) -> dict:
    return {
        "id": rule.id,
        "agency_link_id": rule.agency_link_id,
        "source_agency_id": rule.source_agency_id,
        "source_account_id": rule.source_account_id,
        "destination_agency_id": rule.destination_agency_id,
        "destination_account_id": rule.destination_account_id,
        "source_agency_name": rule.source_agency.name if rule.source_agency else None,
        "source_account_name": rule.source_account.name if rule.source_account else None,
        "destination_agency_name": rule.destination_agency.name if rule.destination_agency else None,
        "destination_account_name": rule.destination_account.name if rule.destination_account else None,
        "name": rule.name,
        "description": rule.description,
        "status": rule.status.value,
        "active": rule.active,
        "created_by_user_id": rule.created_by_user_id,
        "accepted_by_user_id": rule.accepted_by_user_id,
        "created_at": rule.created_at,
        "accepted_at": rule.accepted_at,
    }


def inter_agency_transfer_payload(item: InterAgencyTransfer) -> dict:
    settled_amount = getattr(item, "_settled_amount", Decimal("0")) or Decimal("0")
    remaining_amount = max(Decimal(item.amount) - Decimal(settled_amount), Decimal("0"))
    return {
        "id": item.id,
        "transfer_rule_id": item.transfer_rule_id,
        "source_agency_id": item.source_agency_id,
        "source_account_id": item.source_account_id,
        "destination_agency_id": item.destination_agency_id,
        "destination_account_id": item.destination_account_id,
        "source_agency_name": item.source_agency.name if item.source_agency else None,
        "source_account_name": item.source_account.name if item.source_account else None,
        "destination_agency_name": item.destination_agency.name if item.destination_agency else None,
        "destination_account_name": item.destination_account.name if item.destination_account else None,
        "rule_name": item.rule.name if item.rule else None,
        "amount": item.amount,
        "note": item.note,
        "status": item.status.value,
        "created_by_user_id": item.created_by_user_id,
        "receiver_decision_by_user_id": item.receiver_decision_by_user_id,
        "created_at": item.created_at,
        "decided_at": item.decided_at,
        "settled_amount": settled_amount,
        "remaining_amount": remaining_amount,
    }


def inter_agency_settlement_payload(item: InterAgencySettlement) -> dict:
    return {
        "id": item.id,
        "inter_agency_transfer_id": item.inter_agency_transfer_id,
        "payer_agency_id": item.payer_agency_id,
        "payer_account_id": item.payer_account_id,
        "payer_agency_name": item.payer_agency.name if item.payer_agency else None,
        "payer_account_name": item.payer_account.name if item.payer_account else None,
        "receiver_agency_id": item.receiver_agency_id,
        "receiver_account_id": item.receiver_account_id,
        "receiver_agency_name": item.receiver_agency.name if item.receiver_agency else None,
        "receiver_account_name": item.receiver_account.name if item.receiver_account else None,
        "debt_account_id": item.debt_account_id,
        "debt_account_name": item.debt_account.name if item.debt_account else None,
        "amount": item.amount,
        "note": item.note,
        "status": item.status,
        "account_transfer_id": item.account_transfer_id,
        "created_by_user_id": item.created_by_user_id,
        "accepted_by_user_id": item.accepted_by_user_id,
        "created_at": item.created_at,
        "accepted_at": item.accepted_at,
    }


def attach_inter_agency_settlement_totals(db: Session, rows: list[InterAgencyTransfer]) -> None:
    if not rows:
        return
    transfer_ids = [row.id for row in rows]
    totals = dict(
        db.execute(
            select(
                InterAgencySettlement.inter_agency_transfer_id,
                func.coalesce(func.sum(InterAgencySettlement.amount), 0),
            )
            .where(InterAgencySettlement.inter_agency_transfer_id.in_(transfer_ids))
            .where(InterAgencySettlement.status == "accepted")
            .group_by(InterAgencySettlement.inter_agency_transfer_id)
        ).all()
    )
    for row in rows:
        row._settled_amount = Decimal(totals.get(row.id, 0) or 0)


def inter_agency_settled_amount(db: Session, transfer_id: int) -> Decimal:
    return Decimal(
        db.scalar(
            select(func.coalesce(func.sum(InterAgencySettlement.amount), 0))
            .where(InterAgencySettlement.inter_agency_transfer_id == transfer_id)
            .where(InterAgencySettlement.status == "accepted")
        ) or 0
    )


@app.post("/agency-links", response_model=AgencyLinkOut)
def create_agency_link(payload: AgencyLinkCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_agency_manager(user)
    agency_id = require_agency(user)
    other_agency = db.get(Company, payload.agency_id)
    if not other_agency:
        raise HTTPException(status_code=404, detail="Agency not found")
    if agency_id == payload.agency_id:
        raise HTTPException(status_code=400, detail="Cannot link an agency to itself")
    left, right = sorted([agency_id, payload.agency_id])
    existing = db.scalar(select(AgencyLink).where(AgencyLink.agency_a_id == left, AgencyLink.agency_b_id == right))
    if existing and existing.status in {AgencyLinkStatus.pending, AgencyLinkStatus.active}:
        if existing.status == AgencyLinkStatus.pending and (not existing.requested_agency_id or not existing.target_agency_id):
            existing.requested_agency_id = agency_id
            existing.target_agency_id = payload.agency_id
            existing.requested_by_user_id = user.id
            db.commit()
            db.refresh(existing)
            return link_payload(existing)
        raise HTTPException(status_code=409, detail="Agency link already exists")
    link = existing or AgencyLink(agency_a_id=left, agency_b_id=right)
    link.status = AgencyLinkStatus.pending
    link.requested_agency_id = agency_id
    link.target_agency_id = payload.agency_id
    link.requested_by_user_id = user.id
    link.accepted_by_user_id = None
    link.accepted_at = None
    link.disabled_at = None
    db.add(link)
    write_audit(db, user, AuditArea.company, "link_requested", "Agency link requested", agency_a_id=left, agency_b_id=right)
    db.commit()
    db.refresh(link)
    return link_payload(link)


@app.get("/agency-links", response_model=list[AgencyLinkOut])
def list_agency_links(user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[dict]:
    agency_id = require_agency(user)
    rows = db.scalars(
        select(AgencyLink)
        .where(or_(AgencyLink.agency_a_id == agency_id, AgencyLink.agency_b_id == agency_id))
        .order_by(AgencyLink.created_at.desc(), AgencyLink.id.desc())
    ).all()
    return [link_payload(row) for row in rows]


@app.post("/agency-links/{link_id}/accept", response_model=AgencyLinkOut)
def accept_agency_link(link_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_agency_manager(user)
    agency_id = require_agency(user)
    link = db.get(AgencyLink, link_id)
    if not link or agency_id not in {link.agency_a_id, link.agency_b_id}:
        raise HTTPException(status_code=404, detail="Agency link not found")
    target_agency_id = link_target_agency_id(link, db)
    if target_agency_id != agency_id:
        raise HTTPException(status_code=403, detail="Only the requested agency can approve this link")
    link.status = AgencyLinkStatus.active
    link.accepted_by_user_id = user.id
    link.accepted_at = datetime.now(timezone.utc)
    write_audit(db, user, AuditArea.company, "link_accepted", "Agency link accepted", link_id=link.id)
    db.commit()
    db.refresh(link)
    return link_payload(link)


@app.post("/agency-links/{link_id}/reject", response_model=AgencyLinkOut)
def reject_agency_link(link_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_agency_manager(user)
    agency_id = require_agency(user)
    link = db.get(AgencyLink, link_id)
    if not link or agency_id not in {link.agency_a_id, link.agency_b_id}:
        raise HTTPException(status_code=404, detail="Agency link not found")
    target_agency_id = link_target_agency_id(link, db)
    if target_agency_id != agency_id:
        raise HTTPException(status_code=403, detail="Only the requested agency can reject this link")
    link.status = AgencyLinkStatus.rejected
    link.disabled_at = datetime.now(timezone.utc)
    write_audit(db, user, AuditArea.company, "link_rejected", "Agency link rejected", link_id=link.id)
    db.commit()
    db.refresh(link)
    return link_payload(link)


@app.post("/agency-transfer-rules", response_model=AgencyTransferRuleOut)
def create_agency_transfer_rule(payload: AgencyTransferRuleCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_agency_manager(user)
    agency_id = require_agency(user)
    if payload.source_agency_id != agency_id:
        raise HTTPException(status_code=403, detail="Create rules from your active agency")
    if payload.source_agency_id == payload.destination_agency_id and payload.source_account_id == payload.destination_account_id:
        raise HTTPException(status_code=400, detail="Source and destination cannot be the same account")
    link = db.get(AgencyLink, payload.agency_link_id)
    if not link or link.status != AgencyLinkStatus.active or {link.agency_a_id, link.agency_b_id} != {payload.source_agency_id, payload.destination_agency_id}:
        raise HTTPException(status_code=400, detail="Active agency link is required")
    account_belongs_to_agency(db, payload.source_account_id, payload.source_agency_id)
    account_belongs_to_agency(db, payload.destination_account_id, payload.destination_agency_id)
    rule = AgencyTransferRule(
        agency_link_id=link.id,
        source_agency_id=payload.source_agency_id,
        source_account_id=payload.source_account_id,
        destination_agency_id=payload.destination_agency_id,
        destination_account_id=payload.destination_account_id,
        name=payload.name.strip(),
        description=payload.description,
        status=AgencyTransferRuleStatus.pending,
        active=True,
        created_by_user_id=user.id,
    )
    db.add(rule)
    write_audit(db, user, AuditArea.transaction, "rule_created", "Agency transfer rule created", name=rule.name)
    db.commit()
    db.refresh(rule)
    return rule_payload(rule)


@app.get("/agency-transfer-rules", response_model=list[AgencyTransferRuleOut])
def list_agency_transfer_rules(user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[dict]:
    agency_id = require_agency(user)
    rows = db.scalars(
        select(AgencyTransferRule)
        .where(or_(AgencyTransferRule.source_agency_id == agency_id, AgencyTransferRule.destination_agency_id == agency_id))
        .order_by(AgencyTransferRule.created_at.desc(), AgencyTransferRule.id.desc())
    ).all()
    return [rule_payload(row) for row in rows]


@app.post("/agency-transfer-rules/{rule_id}/accept", response_model=AgencyTransferRuleOut)
def accept_agency_transfer_rule(rule_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_agency_manager(user)
    agency_id = require_agency(user)
    rule = db.get(AgencyTransferRule, rule_id)
    if not rule or rule.destination_agency_id != agency_id:
        raise HTTPException(status_code=404, detail="Transfer rule not found")
    rule.status = AgencyTransferRuleStatus.active
    rule.active = True
    rule.accepted_by_user_id = user.id
    rule.accepted_at = datetime.now(timezone.utc)
    write_audit(db, user, AuditArea.transaction, "rule_accepted", "Agency transfer rule accepted", rule_id=rule.id)
    db.commit()
    db.refresh(rule)
    return rule_payload(rule)


@app.post("/agency-transfer-rules/{rule_id}/reject", response_model=AgencyTransferRuleOut)
def reject_agency_transfer_rule(rule_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_agency_manager(user)
    agency_id = require_agency(user)
    rule = db.get(AgencyTransferRule, rule_id)
    if not rule or rule.destination_agency_id != agency_id:
        raise HTTPException(status_code=404, detail="Transfer rule not found")
    rule.status = AgencyTransferRuleStatus.rejected
    rule.active = False
    rule.disabled_at = datetime.now(timezone.utc)
    write_audit(db, user, AuditArea.transaction, "rule_rejected", "Agency transfer rule rejected", rule_id=rule.id)
    db.commit()
    db.refresh(rule)
    return rule_payload(rule)


@app.post("/inter-agency-transfers", response_model=InterAgencyTransferOut)
def create_inter_agency_transfer(payload: InterAgencyTransferCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_permission(user, db, "accounts", "transfer")
    agency_id = require_agency(user)
    rule = db.get(AgencyTransferRule, payload.transfer_rule_id)
    if not rule or rule.source_agency_id != agency_id:
        raise HTTPException(status_code=404, detail="Transfer rule not found")
    if rule.status != AgencyTransferRuleStatus.active or not rule.active:
        raise HTTPException(status_code=400, detail="Transfer rule is not active")
    if not active_link_between(db, rule.source_agency_id, rule.destination_agency_id):
        raise HTTPException(status_code=400, detail="Agencies are not actively linked")
    account_belongs_to_agency(db, rule.source_account_id, rule.source_agency_id)
    account_belongs_to_agency(db, rule.destination_account_id, rule.destination_agency_id)
    item = InterAgencyTransfer(
        transfer_rule_id=rule.id,
        source_agency_id=rule.source_agency_id,
        source_account_id=rule.source_account_id,
        destination_agency_id=rule.destination_agency_id,
        destination_account_id=rule.destination_account_id,
        amount=payload.amount,
        note=payload.note,
        status=InterAgencyTransferStatus.pending_receiver,
        created_by_user_id=user.id,
    )
    db.add(item)
    write_audit(db, user, AuditArea.transaction, "transfer_created", "Inter-agency transfer created", rule_id=rule.id, amount=str(payload.amount))
    db.commit()
    db.refresh(item)
    return inter_agency_transfer_payload(item)


@app.get("/inter-agency-transfers", response_model=list[InterAgencyTransferOut])
def list_inter_agency_transfers(
    status: str | None = Query(default=None),
    user: User = Depends(current_user),
    db: Session = Depends(get_db),
) -> list[dict]:
    agency_id = require_agency(user)
    query = select(InterAgencyTransfer).where(or_(InterAgencyTransfer.source_agency_id == agency_id, InterAgencyTransfer.destination_agency_id == agency_id))
    if status:
        query = query.where(InterAgencyTransfer.status == status)
    rows = db.scalars(query.order_by(InterAgencyTransfer.created_at.desc(), InterAgencyTransfer.id.desc()).limit(300)).all()
    attach_inter_agency_settlement_totals(db, rows)
    return [inter_agency_transfer_payload(row) for row in rows]


@app.post("/inter-agency-transfers/{transfer_id}/accept", response_model=InterAgencyTransferOut)
def accept_inter_agency_transfer_endpoint(transfer_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    item = db.get(InterAgencyTransfer, transfer_id)
    if not item:
        raise HTTPException(status_code=404, detail="Transfer not found")
    accepted = accept_inter_agency_transfer(db, item, user)
    attach_inter_agency_settlement_totals(db, [accepted])
    return inter_agency_transfer_payload(accepted)


@app.post("/inter-agency-transfers/{transfer_id}/cancel", response_model=InterAgencyTransferOut)
def cancel_inter_agency_transfer(transfer_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    agency_id = require_agency(user)
    item = db.get(InterAgencyTransfer, transfer_id)
    if not item or agency_id not in {item.source_agency_id, item.destination_agency_id}:
        raise HTTPException(status_code=404, detail="Transfer not found")
    if item.status != InterAgencyTransferStatus.pending_receiver:
        raise HTTPException(status_code=409, detail="Transfer is not pending")
    item.status = InterAgencyTransferStatus.cancelled
    item.receiver_decision_by_user_id = user.id
    item.decided_at = datetime.now(timezone.utc)
    write_audit(db, user, AuditArea.transaction, "transfer_cancelled", "Inter-agency transfer cancelled", transfer_id=item.id)
    db.commit()
    db.refresh(item)
    attach_inter_agency_settlement_totals(db, [item])
    return inter_agency_transfer_payload(item)


@app.get("/inter-agency-settlements", response_model=list[InterAgencySettlementOut])
def list_inter_agency_settlements(user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[dict]:
    agency_id = require_agency(user)
    rows = db.scalars(
        select(InterAgencySettlement)
        .where(or_(InterAgencySettlement.payer_agency_id == agency_id, InterAgencySettlement.receiver_agency_id == agency_id))
        .order_by(InterAgencySettlement.created_at.desc(), InterAgencySettlement.id.desc())
        .limit(300)
    ).all()
    return [inter_agency_settlement_payload(row) for row in rows]


@app.post("/inter-agency-settlements", response_model=InterAgencySettlementOut)
def create_inter_agency_settlement(payload: InterAgencySettlementCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_permission(user, db, "accounts", "transfer")
    agency_id = require_agency(user)
    transfer = db.get(InterAgencyTransfer, payload.inter_agency_transfer_id)
    if not transfer or agency_id not in {transfer.source_agency_id, transfer.destination_agency_id}:
        raise HTTPException(status_code=404, detail="Inter-agency transfer not found")
    if transfer.status != InterAgencyTransferStatus.accepted:
        raise HTTPException(status_code=409, detail="Only accepted transfers can be settled")
    if agency_id != transfer.destination_agency_id:
        raise HTTPException(status_code=403, detail="Only the destination agency can return this transfer")

    amount = Decimal(payload.amount)
    payer_account = account_belongs_to_agency(db, payload.payer_account_id, transfer.destination_agency_id)
    receiver_account = account_belongs_to_agency(db, payload.receiver_account_id, transfer.source_agency_id)
    debt_account = account_belongs_to_agency(db, transfer.source_account_id, transfer.source_agency_id)
    occurred_at = datetime.now(timezone.utc)
    settlement = InterAgencySettlement(
        inter_agency_transfer_id=transfer.id,
        payer_agency_id=transfer.destination_agency_id,
        payer_account_id=payer_account.id,
        receiver_agency_id=transfer.source_agency_id,
        receiver_account_id=receiver_account.id,
        debt_account_id=debt_account.id,
        amount=amount,
        note=payload.note,
        status="pending",
        created_by_user_id=user.id,
        created_at=occurred_at,
    )
    db.add(settlement)
    write_audit(db, user, AuditArea.transaction, "settlement_requested", "Inter-agency return requested", transfer_id=transfer.id, settlement_amount=str(amount))
    db.commit()
    db.refresh(settlement)
    return inter_agency_settlement_payload(settlement)


@app.post("/inter-agency-settlements/{settlement_id}/accept", response_model=InterAgencySettlementOut)
def accept_inter_agency_settlement(settlement_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_permission(user, db, "accounts", "transfer")
    agency_id = require_agency(user)
    settlement = db.get(InterAgencySettlement, settlement_id)
    if not settlement or agency_id not in {settlement.payer_agency_id, settlement.receiver_agency_id}:
        raise HTTPException(status_code=404, detail="Settlement request not found")
    if settlement.receiver_agency_id != agency_id:
        raise HTTPException(status_code=403, detail="Only the debt agency can accept this return")
    if settlement.status != "pending":
        raise HTTPException(status_code=409, detail="Settlement is not pending")

    amount = Decimal(settlement.amount)
    occurred_at = datetime.now(timezone.utc)
    note = settlement.note or f"Return payment for inter-agency transfer #{settlement.inter_agency_transfer_id}"
    payer_agency_name = db.scalar(select(Company.name).where(Company.id == settlement.payer_agency_id)) or "Payer agency"
    receiver_agency_name = db.scalar(select(Company.name).where(Company.id == settlement.receiver_agency_id)) or "Receiver agency"
    account_transfer = create_transfer(
        db,
        type("TransferPayload", (), {
            "from_account_id": settlement.payer_account_id,
            "to_account_id": settlement.receiver_account_id,
            "amount": amount,
            "description": note,
            "occurred_at": occurred_at,
            "contributions": [
                {
                    "account_id": settlement.payer_account_id,
                    "agency_id": settlement.receiver_agency_id,
                    "name": receiver_agency_name,
                    "amount": str(amount),
                    "direction": "retrait",
                },
                {
                    "account_id": settlement.receiver_account_id,
                    "agency_id": settlement.payer_agency_id,
                    "name": payer_agency_name,
                    "amount": str(amount),
                    "direction": "versement",
                },
            ],
        })(),
        user,
        commit=False,
    )
    debt_account = account_belongs_to_agency(db, settlement.debt_account_id, settlement.receiver_agency_id)
    apply_balance_delta(
        db,
        debt_account,
        -amount,
        account_transfer_id=account_transfer.id,
        description=f"Debt settlement for transfer #{settlement.inter_agency_transfer_id}",
        occurred_at=occurred_at,
        created_by=user.id,
    )
    settlement.status = "accepted"
    settlement.account_transfer_id = account_transfer.id
    settlement.accepted_by_user_id = user.id
    settlement.accepted_at = occurred_at
    write_audit(db, user, AuditArea.transaction, "settlement_accepted", "Inter-agency return accepted", settlement_id=settlement.id, amount=str(amount))
    db.commit()
    db.refresh(settlement)
    return inter_agency_settlement_payload(settlement)


def account_scoped_contributions(transfer: AccountTransfer, account_id: int) -> list[dict]:
    contributions = [item for item in (transfer.contributions or []) if isinstance(item, dict)]
    has_scoped_rows = any(item.get("account_id") is not None for item in contributions)
    if not has_scoped_rows:
        return contributions
    scoped = []
    for item in contributions:
        try:
            if int(item.get("account_id")) == account_id:
                scoped.append(item)
        except (TypeError, ValueError):
            continue
    return scoped


def account_contribution_total(db: Session, account_id: int) -> Decimal:
    rows = db.scalars(
        select(AccountTransfer)
        .where(
            or_(AccountTransfer.to_account_id == account_id, AccountTransfer.from_account_id == account_id),
            AccountTransfer.contributions.is_not(None),
        )
    ).all()
    total = Decimal("0")
    for row in rows:
        fallback_direction = "versement" if row.to_account_id == account_id else "retrait"
        for contribution in account_scoped_contributions(row, account_id):
            try:
                amount = Decimal(str(contribution.get("amount") or row.amount or 0))
            except Exception:
                amount = Decimal("0")
            direction = contribution.get("direction") or fallback_direction
            total += amount if direction == "versement" else -amount
    return total


def sync_unpaid_account(db: Session, company_id: int | None) -> Decimal:
    settings = app_settings(db, company_id)
    account_id = settings.get("unpaidAccountId") if isinstance(settings, dict) else None
    if not account_id:
        return Decimal("0")
    try:
        account = db.get(Account, int(account_id))
    except (TypeError, ValueError):
        return Decimal("0")
    if not account or (company_id and company_id not in account.company_ids):
        return Decimal("0")
    total = account_contribution_total(db, account.id)
    apply_balance_delta(db, account, total - account_display_balance(account), description="Sync non paye total")
    account.updated_at = datetime.now(timezone.utc)
    return total


def unpaid_target_account(db: Session, company_id: int | None) -> Account | None:
    settings = app_settings(db, company_id)
    account_id = settings.get("unpaidAccountId") if isinstance(settings, dict) else None
    if not account_id:
        return None
    try:
        account = db.get(Account, int(account_id))
    except (TypeError, ValueError):
        return None
    if not account or (company_id and company_id not in account.company_ids):
        return None
    return account


def record_unpaid_contributor(db: Session, user: User, person_name: str, direction: str, amount: Decimal, description: str | None) -> None:
    if amount <= 0:
        return
    account = unpaid_target_account(db, user.company_id)
    if not account:
        return
    apply_balance_delta(db, account, amount if direction == "+" else -amount, description=description or ("Add non paye" if direction == "+" else "Retrieve non paye"), created_by=user.id)
    db.add(
        AccountTransfer(
            from_account_id=account.id if direction == "-" else None,
            to_account_id=account.id if direction == "+" else None,
            amount=amount,
            description=description or ("Add non paye" if direction == "+" else "Retrieve non paye"),
            contributions=[{"name": person_name, "amount": str(amount), "direction": "versement" if direction == "+" else "retrait"}],
            occurred_at=datetime.now(timezone.utc),
            created_by=user.id,
        )
    )


def cash_denomination_key(value: Decimal) -> str:
    formatted = format(Decimal(value), "f")
    if "." not in formatted:
        return formatted
    return formatted.rstrip("0").rstrip(".")


def active_unpaid_person(db: Session, person_name: str) -> UnpaidItem | None:
    return db.scalar(
        select(UnpaidItem).where(
            func.lower(UnpaidItem.person_name) == person_name.strip().lower(),
            UnpaidItem.settled.is_(False),
        )
    )


def require_fixed_transfer_rules(payload: TransferCreate, user: User, db: Session) -> None:
    if user.company_id:
        for account_id in [payload.from_account_id, payload.to_account_id]:
            if account_id is None:
                continue
            account = db.get(Account, account_id)
            if not account_is_accessible(account, user, db):
                raise HTTPException(status_code=403, detail="Account does not belong to active agency")

    settings = accounts_screen_settings(db, user.company_id)
    if not payload.context_account_id:
        raise HTTPException(status_code=403, detail="Transfer context is required")
    context_account = db.get(Account, payload.context_account_id)
    if not account_is_accessible(context_account, user, db):
        raise HTTPException(status_code=403, detail="Transfer context account is invalid")

    account_config = settings.get(str(payload.context_account_id), {}) if isinstance(settings, dict) else {}
    popups = account_config.get("popups", {}) if isinstance(account_config, dict) else {}
    movement = popups.get("movement", {}) if isinstance(popups, dict) else {}
    transfer_config = popups.get("transfer", {}) if isinstance(popups, dict) else {}

    is_between_accounts = bool(payload.from_account_id and payload.to_account_id)
    if is_between_accounts:
        fixed_from = str(transfer_config.get("fixedFromAccountId") or "")
        fixed_to = str(transfer_config.get("fixedToAccountId") or "")
        if transfer_config.get("applyFixedFromAccount") and str(payload.from_account_id) != fixed_from:
            raise HTTPException(status_code=403, detail="Fixed source account cannot be changed")
        if transfer_config.get("applyFixedToAccount") and str(payload.to_account_id) != fixed_to:
            raise HTTPException(status_code=403, detail="Fixed target account cannot be changed")
        return

    fixed_account = str(movement.get("fixedAccountId") or "")
    if movement.get("applyFixedAccount"):
        account_id = payload.from_account_id or payload.to_account_id
        if str(account_id) != fixed_account:
            raise HTTPException(status_code=403, detail="Fixed account cannot be changed")

    if movement.get("applyFixedType"):
        fixed_type = movement.get("fixedType")
        if fixed_type == "versement" and (payload.from_account_id is not None or payload.to_account_id is None):
            raise HTTPException(status_code=403, detail="Fixed operation type cannot be changed")
        if fixed_type == "retrait" and (payload.to_account_id is not None or payload.from_account_id is None):
            raise HTTPException(status_code=403, detail="Fixed operation type cannot be changed")


@app.get("/settings/accounts-screen")
def get_accounts_screen_settings(user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    return accounts_screen_settings(db, user.company_id)


@app.patch("/settings/accounts-screen")
def save_accounts_screen_settings(payload: AccountsScreenSettingsIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    require_permission(user, db, "account-settings", "configure")
    if not user.company_id:
        raise HTTPException(status_code=400, detail="No active agency")
    db.execute(
        text("""
            INSERT INTO agency_settings (company_id, key, value, updated_at)
            VALUES (:company_id, 'accounts_screen_config', CAST(:value AS jsonb), now())
            ON CONFLICT (company_id, key)
            DO UPDATE SET value = EXCLUDED.value, updated_at = now()
        """),
        {"company_id": user.company_id, "value": json.dumps(payload.config)},
    )
    db.commit()
    return payload.config


@app.get("/settings/account-actions")
def get_account_action_settings(user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    return account_action_settings(db, user.company_id)


@app.patch("/settings/account-actions")
def save_account_action_settings(payload: AccountsScreenSettingsIn, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> dict:
    if not user.company_id:
        raise HTTPException(status_code=400, detail="No active agency")
    db.execute(
        text("""
            INSERT INTO agency_settings (company_id, key, value, updated_at)
            VALUES (:company_id, 'account_action_rules', CAST(:value AS jsonb), now())
            ON CONFLICT (company_id, key)
            DO UPDATE SET value = EXCLUDED.value, updated_at = now()
        """),
        {"company_id": user.company_id, "value": json.dumps(payload.config)},
    )
    db.commit()
    return payload.config


@app.get("/settings/app")
def get_app_settings(user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    return app_settings(db, user.company_id)


@app.patch("/settings/app")
def save_app_settings(payload: AccountsScreenSettingsIn, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> dict:
    if not user.company_id:
        raise HTTPException(status_code=400, detail="No active agency")
    db.execute(
        text("""
            INSERT INTO agency_settings (company_id, key, value, updated_at)
            VALUES (:company_id, 'app_settings', CAST(:value AS jsonb), now())
            ON CONFLICT (company_id, key)
            DO UPDATE SET value = EXCLUDED.value, updated_at = now()
        """),
        {"company_id": user.company_id, "value": json.dumps(payload.config)},
    )
    db.commit()
    return payload.config


@app.get("/register-clients")
def get_register_clients(user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    if not user.company_id:
        return {"clients": []}
    value = db.execute(
        text("SELECT value FROM agency_settings WHERE company_id = :company_id AND key = 'register_clients'"),
        {"company_id": user.company_id},
    ).scalar()
    return value if isinstance(value, dict) else {"clients": []}


@app.patch("/register-clients")
def save_register_clients(payload: AccountsScreenSettingsIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    if not user.company_id:
        raise HTTPException(status_code=400, detail="No active agency")
    db.execute(
        text("""
            INSERT INTO agency_settings (company_id, key, value, updated_at)
            VALUES (:company_id, 'register_clients', CAST(:value AS jsonb), now())
            ON CONFLICT (company_id, key)
            DO UPDATE SET value = EXCLUDED.value, updated_at = now()
        """),
        {"company_id": user.company_id, "value": json.dumps(payload.config)},
    )
    db.commit()
    return payload.config


@app.get("/dashboard", response_model=DashboardSummary)
def dashboard(user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    return dashboard_summary(db, user.company_id, hidden_account_ids(db, user))


@app.get("/reports")
def reports(
    from_date: date = Query(...),
    to_date: date = Query(...),
    user: User = Depends(current_user),
    db: Session = Depends(get_db),
) -> dict:
    if to_date < from_date:
        raise HTTPException(status_code=400, detail="To date must be after from date")
    query = (
        select(ServiceTransaction, Service.name)
        .join(Service, Service.id == ServiceTransaction.service_id)
        .join(User, User.id == ServiceTransaction.created_by)
        .where(
            func.date(ServiceTransaction.occurred_at) >= from_date,
            func.date(ServiceTransaction.occurred_at) <= to_date,
            ServiceTransaction.reversed_at.is_(None),
        )
    )
    if user.company_id:
        query = query.where(User.company_id == user.company_id)
    rows = db.execute(query.order_by(ServiceTransaction.occurred_at.desc(), ServiceTransaction.id.desc())).all()
    service_totals: dict[str, dict[str, Decimal | int]] = {}
    daily_totals: dict[str, dict[str, Decimal | int]] = {}
    total_in = Decimal("0")
    total_out = Decimal("0")
    total_fees = Decimal("0")
    items = []
    for tx, service_name in rows:
        amount = Decimal(tx.amount)
        fee = Decimal(tx.fee or 0)
        day = tx.occurred_at.date().isoformat()
        direction = tx.direction.value
        if direction == "IN":
            total_in += amount
        else:
            total_out += amount
        total_fees += fee
        service_bucket = service_totals.setdefault(service_name, {"IN": Decimal("0"), "OUT": Decimal("0"), "fees": Decimal("0"), "count": 0})
        day_bucket = daily_totals.setdefault(day, {"IN": Decimal("0"), "OUT": Decimal("0"), "fees": Decimal("0"), "count": 0})
        service_bucket[direction] = Decimal(service_bucket[direction]) + amount
        service_bucket["fees"] = Decimal(service_bucket["fees"]) + fee
        service_bucket["count"] = int(service_bucket["count"]) + 1
        day_bucket[direction] = Decimal(day_bucket[direction]) + amount
        day_bucket["fees"] = Decimal(day_bucket["fees"]) + fee
        day_bucket["count"] = int(day_bucket["count"]) + 1
        items.append({
            "id": tx.id,
            "service": service_name,
            "direction": direction,
            "amount": amount,
            "fee": fee,
            "description": tx.description,
            "occurred_at": tx.occurred_at,
        })
    return {
        "from_date": from_date,
        "to_date": to_date,
        "kpis": {
            "total_in": total_in,
            "total_out": total_out,
            "fees": total_fees,
            "net": total_in - total_out + total_fees,
            "count": len(items),
            "average": (total_in + total_out) / len(items) if items else Decimal("0"),
        },
        "by_service": [{"service": name, **values} for name, values in service_totals.items()],
        "by_day": [{"date": day, **values} for day, values in sorted(daily_totals.items())],
        "rows": items,
    }


@app.get("/agency-ledger", response_model=list[AgencyLedgerEntryOut])
def agency_ledger(
    from_date: date | None = Query(default=None),
    to_date: date | None = Query(default=None),
    user: User = Depends(current_user),
    db: Session = Depends(get_db),
) -> list[AgencyLedgerEntry]:
    query = select(AgencyLedgerEntry).join(User, User.id == AgencyLedgerEntry.created_by)
    if user.company_id:
        query = query.where(User.company_id == user.company_id)
    if from_date:
        query = query.where(func.date(AgencyLedgerEntry.occurred_at) >= from_date)
    if to_date:
        query = query.where(func.date(AgencyLedgerEntry.occurred_at) <= to_date)
    return list(db.scalars(query.order_by(AgencyLedgerEntry.occurred_at.desc(), AgencyLedgerEntry.id.desc()).limit(500)))


@app.post("/agency-ledger", response_model=AgencyLedgerEntryOut)
def create_agency_ledger_entry(payload: AgencyLedgerEntryIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> AgencyLedgerEntry:
    item = AgencyLedgerEntry(
        kind=payload.kind,
        category=payload.category.strip(),
        amount=payload.amount,
        description=payload.description.strip() if payload.description else None,
        occurred_at=payload.occurred_at or datetime.now(timezone.utc),
        created_by=user.id,
    )
    db.add(item)
    write_audit(db, user, AuditArea.transaction, "create", "Agency ledger entry", kind=item.kind, category=item.category, amount=str(item.amount))
    db.commit()
    db.refresh(item)
    return item


@app.get("/accounts", response_model=list[AccountOut])
def list_accounts(user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[Account]:
    query = select(Account)
    if user.company_id:
        query = query.where(Account.company_ids.any(user.company_id))
    accounts = list(db.scalars(query.order_by(Account.legacy_id.nullslast(), Account.name)))
    for account in accounts:
        sync_account_balance(account)
    if user.role == UserRole.admin:
        return accounts
    hidden_ids = hidden_account_ids(db, user)
    return [account for account in accounts if account.visible and account.id not in hidden_ids]


@app.get("/agencies/{agency_id}/accounts", response_model=list[AccountOut])
def list_linked_agency_accounts(agency_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[Account]:
    active_agency_id = require_agency(user)
    if agency_id != active_agency_id and not active_link_between(db, active_agency_id, agency_id):
        raise HTTPException(status_code=403, detail="Active agency link is required")
    query = select(Account).where(Account.company_ids.any(agency_id), Account.visible.is_(True))
    return list(db.scalars(query.order_by(Account.legacy_id.nullslast(), Account.name)))


@app.post("/accounts", response_model=AccountOut)
def create_account(payload: AccountCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> Account:
    require_permission(user, db, "accounts", "create")
    account = Account(
        name=payload.name,
        balance=0,
        previous_balance=0,
        debit_total=0,
        credit_total=0,
        normal_balance_side=payload.normal_balance_side,
        visible=payload.visible,
        company_ids=[user.company_id] if user.company_id else [],
    )
    db.add(account)
    db.flush()
    if Decimal(payload.balance) != 0:
        apply_balance_delta(db, account, Decimal(payload.balance), description="Opening balance", created_by=user.id)
    db.commit()
    db.refresh(account)
    return account


@app.patch("/accounts/{account_id}/balance", response_model=AccountOut)
def update_account_balance(account_id: int, payload: AccountBalanceUpdate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> Account:
    require_permission(user, db, "accounts", "changeBalance")
    account = require_account_access(db.get(Account, account_id), user, db)
    sync_account_balance(account)
    if payload.normal_balance_side and payload.normal_balance_side != account.normal_balance_side:
        reset_account_opening_balance(account, account.balance, payload.normal_balance_side)
    apply_balance_delta(db, account, Decimal(payload.balance) - account_display_balance(account), description="Manual balance adjustment", created_by=user.id)
    account.updated_at = datetime.now(timezone.utc)
    db.commit()
    db.refresh(account)
    return account


@app.post("/transfers")
def transfer(payload: TransferCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, int]:
    require_fixed_transfer_rules(payload, user, db)
    item = create_transfer(db, payload, user)
    return {"id": item.id}


@app.get("/accounts/{account_id}/contributions")
def account_contributions(account_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[dict]:
    require_account_access(db.get(Account, account_id), user, db)
    rows = db.scalars(
        select(AccountTransfer)
        .where(
            or_(AccountTransfer.to_account_id == account_id, AccountTransfer.from_account_id == account_id),
            AccountTransfer.contributions.is_not(None),
        )
        .order_by(AccountTransfer.occurred_at.desc(), AccountTransfer.id.desc())
    ).all()
    result = []
    for row in rows:
        contributions = account_scoped_contributions(row, account_id)
        if not contributions:
            continue
        result.append({
            "id": row.id,
            "account_id": account_id,
            "amount": row.amount,
            "direction": "versement" if row.to_account_id == account_id else "retrait",
            "description": row.description,
            "occurred_at": row.occurred_at,
            "contributions": contributions,
        })
    return result


@app.get("/accounts/{account_id}/movements")
def account_movements(account_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[dict]:
    account = require_account_access(db.get(Account, account_id), user, db)
    sync_account_balance(account)
    rows = db.scalars(
        select(AccountLedgerEntry)
        .where(AccountLedgerEntry.account_id == account_id)
        .order_by(AccountLedgerEntry.occurred_at.desc(), AccountLedgerEntry.id.desc())
        .limit(250)
    ).all()
    transfer_ids = {row.account_transfer_id for row in rows if row.account_transfer_id}
    transfers = db.scalars(select(AccountTransfer).where(AccountTransfer.id.in_(transfer_ids))).all() if transfer_ids else []
    transfer_by_id = {item.id: item for item in transfers}
    related_ids = {row.from_account_id for row in transfers if row.from_account_id} | {row.to_account_id for row in transfers if row.to_account_id}
    related_accounts = db.scalars(select(Account).where(Account.id.in_(related_ids))).all() if related_ids else []
    account_names = {item.id: item.name for item in related_accounts}
    result = []
    for row in rows:
        transfer = transfer_by_id.get(row.account_transfer_id) if row.account_transfer_id else None
        debit_amount = row.amount if row.side == "debit" else Decimal("0")
        credit_amount = row.amount if row.side == "credit" else Decimal("0")
        result.append({
            "id": row.id,
            "account_id": account_id,
            "amount": row.amount,
            "direction": "in" if Decimal(row.balance_effect) >= 0 else "out",
            "debit": debit_amount,
            "credit": credit_amount,
            "balance_effect": row.balance_effect,
            "balance_after": row.balance_after,
            "side": row.side,
            "description": row.description,
            "occurred_at": row.occurred_at,
            "from_account_id": transfer.from_account_id if transfer else None,
            "to_account_id": transfer.to_account_id if transfer else None,
            "from_account_name": account_names.get(transfer.from_account_id) if transfer and transfer.from_account_id else None,
            "to_account_name": account_names.get(transfer.to_account_id) if transfer and transfer.to_account_id else None,
            "contributions": account_scoped_contributions(transfer, account_id) if transfer and transfer.contributions else [],
        })
    return result


@app.get("/services", response_model=list[ServiceOut])
def list_services(user: User = Depends(current_user), db: Session = Depends(get_db)) -> list[Service]:
    query = select(Service)
    if user.company_id:
        account_ids = select(Account.id).where(Account.company_ids.any(user.company_id))
        query = query.where(
            or_(
                Service.company_id == user.company_id,
                Service.primary_account_id.in_(account_ids),
                Service.secondary_account_id.in_(account_ids),
            )
        )
    return list(db.scalars(query.order_by(Service.name)))


def normalize_service_type(value: str) -> str:
    service_type = value.strip().upper().replace("IN AND OUT", "IN & OUT")
    if service_type not in {"IN", "OUT", "IN & OUT"}:
        raise HTTPException(status_code=400, detail="Service type must be IN, OUT, or IN & OUT")
    return service_type


def service_type_directions(service_type: str) -> set[str]:
    if service_type == "IN & OUT":
        return {"IN", "OUT"}
    if "IN" in service_type and "OUT" in service_type:
        return {"IN", "OUT"}
    return {service_type}


def validate_service_routing(db: Session, company_id: int | None, service_type: str, routing_config: dict | None) -> dict | None:
    if routing_config is None:
        return None
    normalized: dict[str, dict[str, int]] = {}
    for direction in service_type_directions(service_type):
        route = routing_config.get(direction) if isinstance(routing_config, dict) else None
        if not isinstance(route, dict):
            continue
        try:
            from_account_id = int(route.get("from_account_id") or 0)
            to_account_id = int(route.get("to_account_id") or 0)
        except (TypeError, ValueError):
            raise HTTPException(status_code=400, detail=f"{direction} routing accounts are invalid")
        if not from_account_id or not to_account_id:
            continue
        for account_id in [from_account_id, to_account_id]:
            account = db.get(Account, account_id)
            if not account or (company_id and company_id not in account.company_ids):
                raise HTTPException(status_code=403, detail=f"{direction} routing account does not belong to active agency")
        normalized[direction] = {"from_account_id": from_account_id, "to_account_id": to_account_id}
    return normalized


@app.post("/services", response_model=ServiceOut)
def create_service(payload: ServiceCreate, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> Service:
    service_type = normalize_service_type(payload.transaction_type)
    service = Service(
        company_id=user.company_id,
        name=payload.name.strip(),
        image_url=payload.image_url.strip() if payload.image_url else None,
        transaction_type=service_type,
        routing_config=validate_service_routing(db, user.company_id, service_type, payload.routing_config),
        active=payload.active,
    )
    db.add(service)
    db.commit()
    db.refresh(service)
    return service


@app.patch("/services/{service_id}", response_model=ServiceOut)
def update_service(service_id: int, payload: ServiceUpdate, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> Service:
    service = db.get(Service, service_id)
    if not service:
        raise HTTPException(status_code=404, detail="Service not found")
    if user.company_id:
        account_ids = select(Account.id).where(Account.company_ids.any(user.company_id))
        belongs_to_agency = (
            service.company_id == user.company_id
            or db.scalar(
                select(Service.id).where(
                    Service.id == service_id,
                    or_(Service.primary_account_id.in_(account_ids), Service.secondary_account_id.in_(account_ids)),
                )
            )
        )
        if not belongs_to_agency:
            raise HTTPException(status_code=404, detail="Service not found")

    service_type = normalize_service_type(payload.transaction_type)
    service.name = payload.name.strip()
    service.image_url = payload.image_url.strip() if payload.image_url else None
    service.transaction_type = service_type
    service.switch_type = None
    if payload.routing_config is not None:
        service.routing_config = validate_service_routing(db, user.company_id, service_type, payload.routing_config)
    service.active = payload.active
    db.commit()
    db.refresh(service)
    return service


@app.delete("/services/{service_id}")
def delete_service(service_id: int, user: User = Depends(require_admin), db: Session = Depends(get_db)) -> dict[str, bool]:
    service = db.get(Service, service_id)
    if not service:
        raise HTTPException(status_code=404, detail="Service not found")
    if user.company_id:
        account_ids = select(Account.id).where(Account.company_ids.any(user.company_id))
        belongs_to_agency = (
            service.company_id == user.company_id
            or db.scalar(
                select(Service.id).where(
                    Service.id == service_id,
                    or_(Service.primary_account_id.in_(account_ids), Service.secondary_account_id.in_(account_ids)),
                )
            )
        )
        if not belongs_to_agency:
            raise HTTPException(status_code=404, detail="Service not found")
    has_history = db.scalar(select(ServiceTransaction.id).where(ServiceTransaction.service_id == service_id).limit(1))
    if has_history:
        raise HTTPException(status_code=409, detail="Service has transaction history and cannot be deleted")
    db.delete(service)
    db.commit()
    return {"deleted": True}


@app.post("/service-fees")
def create_fee_rule(payload: FeeRuleCreate, _: User = Depends(require_admin), db: Session = Depends(get_db)) -> dict[str, int]:
    rule = ServiceFeeRule(**payload.model_dump())
    db.add(rule)
    db.commit()
    db.refresh(rule)
    return {"id": rule.id}


def service_transaction_payload(tx: ServiceTransaction) -> dict:
    return {
        "id": tx.id,
        "service_id": tx.service_id,
        "direction": tx.direction.value,
        "amount": tx.amount,
        "fee": tx.fee,
        "description": tx.description,
        "occurred_at": tx.occurred_at,
    }


def owned_service_transaction(db: Session, transaction_id: int, user: User) -> ServiceTransaction:
    tx = db.get(ServiceTransaction, transaction_id)
    if not tx or tx.reversed_at is not None:
        raise HTTPException(status_code=404, detail="Transaction not found")
    creator = db.get(User, tx.created_by) if tx.created_by else None
    if user.company_id and (not creator or creator.company_id != user.company_id):
        raise HTTPException(status_code=404, detail="Transaction not found")
    return tx


@app.get("/service-transactions")
def list_service_transactions(
    service_id: int = Query(...),
    occurred_on: date | None = Query(default=None),
    user: User = Depends(current_user),
    db: Session = Depends(get_db),
) -> list[dict]:
    service = db.get(Service, service_id)
    if not service:
        raise HTTPException(status_code=404, detail="Service not found")
    day = occurred_on or date.today()
    query = (
        select(ServiceTransaction)
        .join(User, User.id == ServiceTransaction.created_by)
        .where(
            ServiceTransaction.service_id == service_id,
            func.date(ServiceTransaction.occurred_at) == day,
            ServiceTransaction.reversed_at.is_(None),
        )
    )
    if user.company_id:
        query = query.where(User.company_id == user.company_id)
    rows = db.scalars(query.order_by(ServiceTransaction.id)).all()
    return [service_transaction_payload(row) for row in rows]


@app.post("/service-transactions")
def service_transaction(payload: ServiceTransactionCreate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, int]:
    item = create_service_transaction(db, payload, user)
    return {"id": item.id}


@app.patch("/service-transactions/{transaction_id}")
def patch_service_transaction(transaction_id: int, payload: ServiceTransactionUpdate, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    tx = owned_service_transaction(db, transaction_id, user)
    return service_transaction_payload(update_service_transaction(db, tx, payload.amount, payload.fee, user))


@app.delete("/service-transactions/{transaction_id}")
def delete_service_transaction(transaction_id: int, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, bool]:
    tx = owned_service_transaction(db, transaction_id, user)
    reverse_service_transaction(db, tx, user)
    return {"deleted": True}


@app.post("/service-transactions/import-ai")
async def import_service_transactions(
    file: UploadFile = File(...),
    mode: str = Form(default="ai"),
    user: User = Depends(current_user),
    db: Session = Depends(get_db),
) -> dict:
    service_query = select(Service).where(Service.active.is_(True))
    if user.company_id:
        service_query = service_query.where(or_(Service.company_id == user.company_id, Service.company_id.is_(None)))
    services = db.scalars(service_query.order_by(Service.name)).all()
    content = await file.read()
    rows = spreadsheet_rows(file.filename or "", content)
    if not rows:
        raise HTTPException(status_code=400, detail="No rows found in import file")
    print(table_excerpt(rows), flush=True)
    allowed_directions = ["IN", "OUT"]
    ai_config = app_settings(db, user.company_id) if user.company_id else {}
    service_ids = {service.name: service.id for service in services}
    if mode == "manual":
        raw_rules = ai_config.get("manualImportRules") if isinstance(ai_config, dict) else []
        transformed_rows = manual_transform_transactions(
            rows,
            raw_rules if isinstance(raw_rules, list) else [],
            [
                {
                    "id": service.id,
                    "name": service.name,
                    "transaction_type": service.transaction_type,
                    "switch_type": service.switch_type,
                }
                for service in services
            ],
            allowed_directions,
        )
        for row in transformed_rows:
            row["service_id"] = service_ids.get(row.get("service"))
        return {"rows": transformed_rows, "raw_rows": len(rows), "mode": "manual"}

    provider = ai_config.get("aiProvider") if ai_config.get("aiProvider") in {"openai", "google_gemini"} else "openai"
    prompt_config = ai_config.get("openaiImportPrompt")
    api_key = ai_config.get("geminiApiKey" if provider == "google_gemini" else "openaiApiKey")
    model = ai_config.get("geminiModel" if provider == "google_gemini" else "openaiModel")
    if not isinstance(model, str) or not model.strip():
        model = DEFAULT_GEMINI_MODEL if provider == "google_gemini" else DEFAULT_OPENAI_MODEL
    service_types = {service.name: service.transaction_type or service.switch_type or "IN & OUT" for service in services}
    transformed_rows = transform_transactions(
        provider,
        rows,
        [service.name for service in services],
        allowed_directions,
        prompt_config if isinstance(prompt_config, str) else None,
        api_key if isinstance(api_key, str) else None,
        model,
        service_types,
    )
    for row in transformed_rows:
        row["service_id"] = service_ids.get(row.get("service"))
    return {"rows": transformed_rows, "raw_rows": len(rows), "mode": "ai"}


@app.post("/cash-counts")
def cash_count(payload: CashCountIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    return save_cash_count(db, payload, user)


@app.get("/cash-counts/{counted_on}")
def get_cash_count(counted_on: date, _: User = Depends(current_user), db: Session = Depends(get_db)) -> dict:
    rows = db.scalars(select(CashCount).where(CashCount.counted_on == counted_on)).all()
    counts = {cash_denomination_key(row.denomination): row.quantity for row in rows}
    total = sum((Decimal(row.denomination) * row.quantity for row in rows), Decimal("0"))
    return {"counted_on": counted_on, "counts": counts, "total": total}


@app.get("/unpaid-items", response_model=list[UnpaidItemOut])
def unpaid_items(_: User = Depends(current_user), db: Session = Depends(get_db)) -> list[UnpaidItem]:
    return list(db.scalars(select(UnpaidItem).order_by(UnpaidItem.id.desc())))


@app.post("/unpaid-items")
def create_unpaid(payload: UnpaidItemIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, int]:
    person_name = payload.person_name.strip()
    item = active_unpaid_person(db, person_name)
    amount = Decimal(payload.amount)
    if item:
        item.amount = Decimal(item.amount) + amount
        item.description = payload.description or item.description
        person_name = item.person_name
    else:
        item = UnpaidItem(person_name=person_name, amount=payload.amount, description=payload.description, settled=False)
        db.add(item)
        db.flush()
    if amount > 0:
        db.add(UnpaidMovement(person_name=person_name, direction="+", amount=payload.amount, description=payload.description, created_by=user.id))
        record_unpaid_contributor(db, user, person_name, "+", amount, payload.description)
    db.commit()
    db.refresh(item)
    return {"id": item.id}


@app.post("/unpaid-movements")
def create_unpaid_movement(payload: UnpaidMovementIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, int]:
    person_name = payload.person_name.strip()
    item = active_unpaid_person(db, person_name)
    if not item:
        item = UnpaidItem(person_name=person_name, amount=0, description=payload.description, settled=False)
        db.add(item)
        db.flush()
    delta = Decimal(payload.amount) if payload.direction == "+" else -Decimal(payload.amount)
    item.amount = Decimal(item.amount) + delta
    item.description = payload.description or item.description
    movement = UnpaidMovement(person_name=item.person_name, direction=payload.direction, amount=payload.amount, description=payload.description, created_by=user.id)
    db.add(movement)
    record_unpaid_contributor(db, user, item.person_name, payload.direction, Decimal(payload.amount), payload.description)
    db.commit()
    db.refresh(movement)
    return {"id": movement.id}


@app.post("/unpaid-items/sync-account")
def sync_unpaid_total(user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, Decimal]:
    total = sync_unpaid_account(db, user.company_id)
    db.commit()
    return {"total": total}


@app.get("/unpaid-movements/{person_name}")
def unpaid_history(person_name: str, _: User = Depends(current_user), db: Session = Depends(get_db)) -> list[dict]:
    rows = db.scalars(
        select(UnpaidMovement)
        .where(UnpaidMovement.person_name == person_name)
        .order_by(UnpaidMovement.occurred_at.desc(), UnpaidMovement.id.desc())
        .limit(200)
    ).all()
    return [
        {
            "id": row.id,
            "person_name": row.person_name,
            "direction": row.direction,
            "amount": row.amount,
            "description": row.description,
            "occurred_at": row.occurred_at,
        }
        for row in rows
    ]


@app.post("/salaf")
def salaf(payload: SalafEntryIn, user: User = Depends(current_user), db: Session = Depends(get_db)) -> dict[str, int]:
    item = create_salaf(db, payload, user)
    return {"id": item.id}


@app.get("/audit")
def audit(_: User = Depends(require_admin), db: Session = Depends(get_db)) -> list[dict]:
    rows = db.scalars(select(AuditLog).order_by(AuditLog.created_at.desc()).limit(100)).all()
    return [{"id": row.id, "area": row.area.value, "action": row.action, "title": row.title, "user": row.user_name, "created_at": row.created_at} for row in rows]
