from datetime import date, datetime
from decimal import Decimal
from enum import Enum

from sqlalchemy import ARRAY, BigInteger, Boolean, Date, DateTime, Enum as SqlEnum, ForeignKey, Integer, Numeric, Text, func
from sqlalchemy.dialects.postgresql import JSONB
from sqlalchemy.orm import Mapped, mapped_column, relationship

from app.db import Base


class UserRole(str, Enum):
    admin = "Admin"
    chef = "Chef"
    user = "User"


class Direction(str, Enum):
    in_ = "IN"
    out = "OUT"


class AgencyLinkStatus(str, Enum):
    pending = "pending"
    active = "active"
    rejected = "rejected"
    disabled = "disabled"


class AgencyTransferRuleStatus(str, Enum):
    pending = "pending"
    active = "active"
    rejected = "rejected"
    disabled = "disabled"


class InterAgencyTransferStatus(str, Enum):
    pending_receiver = "pending_receiver"
    accepted = "accepted"
    cancelled = "cancelled"
    rejected = "rejected"


class AuditArea(str, Enum):
    auth = "auth"
    user = "user"
    company = "company"
    account = "account"
    service = "service"
    transaction = "transaction"
    cash = "cash"
    register = "register"
    report = "report"


class Company(Base):
    __tablename__ = "companies"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    name: Mapped[str] = mapped_column(Text, unique=True)
    description: Mapped[str | None] = mapped_column(Text)
    seo: Mapped[str | None] = mapped_column(Text)
    chef: Mapped[str | None] = mapped_column(Text)
    admin_local: Mapped[str | None] = mapped_column(Text)
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    updated_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())


class User(Base):
    __tablename__ = "users"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    company_id: Mapped[int | None] = mapped_column(ForeignKey("companies.id"))
    username: Mapped[str] = mapped_column(Text, unique=True)
    password_hash: Mapped[str] = mapped_column(Text)
    first_name: Mapped[str] = mapped_column(Text)
    last_name: Mapped[str] = mapped_column(Text)
    email: Mapped[str | None] = mapped_column(Text)
    phone: Mapped[str | None] = mapped_column(Text)
    info: Mapped[str | None] = mapped_column(Text)
    role: Mapped[UserRole] = mapped_column(SqlEnum(UserRole, name="user_role", values_callable=lambda x: [e.value for e in x]))
    permissions: Mapped[dict | None] = mapped_column(JSONB)
    image_url: Mapped[str | None] = mapped_column(Text)
    active: Mapped[bool] = mapped_column(Boolean, default=True)
    company: Mapped[Company | None] = relationship()


class Account(Base):
    __tablename__ = "accounts"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    name: Mapped[str] = mapped_column(Text, unique=True)
    balance: Mapped[Decimal] = mapped_column(Numeric(14, 2), default=0)
    previous_balance: Mapped[Decimal | None] = mapped_column(Numeric(14, 2))
    debit_total: Mapped[Decimal] = mapped_column(Numeric(14, 2), default=0)
    credit_total: Mapped[Decimal] = mapped_column(Numeric(14, 2), default=0)
    normal_balance_side: Mapped[str] = mapped_column(Text, default="debit")
    visible: Mapped[bool] = mapped_column(Boolean, default=True)
    company_ids: Mapped[list[int]] = mapped_column(ARRAY(BigInteger), default=list)
    legacy_id: Mapped[int | None] = mapped_column(Integer, unique=True)
    updated_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())


class AccountTransfer(Base):
    __tablename__ = "account_transfers"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    from_account_id: Mapped[int | None] = mapped_column(ForeignKey("accounts.id"))
    to_account_id: Mapped[int | None] = mapped_column(ForeignKey("accounts.id"))
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    description: Mapped[str | None] = mapped_column(Text)
    contributions: Mapped[list[dict] | None] = mapped_column(JSONB)
    is_paid: Mapped[bool] = mapped_column(Boolean, default=False)
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    import_batch_id: Mapped[str | None] = mapped_column(Text)
    import_source: Mapped[str | None] = mapped_column(Text)
    reversed_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    reversal_reason: Mapped[str | None] = mapped_column(Text)


class AccountLedgerEntry(Base):
    __tablename__ = "account_ledger_entries"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    account_transfer_id: Mapped[int | None] = mapped_column(ForeignKey("account_transfers.id"))
    side: Mapped[str] = mapped_column(Text)
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    balance_effect: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    balance_after: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    description: Mapped[str | None] = mapped_column(Text)
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    account: Mapped[Account] = relationship()
    account_transfer: Mapped[AccountTransfer | None] = relationship()


class AgencyLink(Base):
    __tablename__ = "agency_links"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    agency_a_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    agency_b_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    status: Mapped[AgencyLinkStatus] = mapped_column(SqlEnum(AgencyLinkStatus, name="agency_link_status", values_callable=lambda x: [e.value for e in x]), default=AgencyLinkStatus.pending)
    requested_agency_id: Mapped[int | None] = mapped_column(ForeignKey("companies.id"))
    target_agency_id: Mapped[int | None] = mapped_column(ForeignKey("companies.id"))
    requested_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    accepted_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    accepted_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    disabled_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    agency_a: Mapped[Company] = relationship(foreign_keys=[agency_a_id])
    agency_b: Mapped[Company] = relationship(foreign_keys=[agency_b_id])
    requested_agency: Mapped[Company | None] = relationship(foreign_keys=[requested_agency_id])
    target_agency: Mapped[Company | None] = relationship(foreign_keys=[target_agency_id])


class AgencyTransferRule(Base):
    __tablename__ = "agency_transfer_rules"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    agency_link_id: Mapped[int] = mapped_column(ForeignKey("agency_links.id"))
    source_agency_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    source_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    destination_agency_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    destination_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    name: Mapped[str] = mapped_column(Text)
    description: Mapped[str | None] = mapped_column(Text)
    status: Mapped[AgencyTransferRuleStatus] = mapped_column(SqlEnum(AgencyTransferRuleStatus, name="agency_transfer_rule_status", values_callable=lambda x: [e.value for e in x]), default=AgencyTransferRuleStatus.pending)
    active: Mapped[bool] = mapped_column(Boolean, default=True)
    created_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    accepted_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    accepted_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    disabled_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    link: Mapped[AgencyLink] = relationship()
    source_agency: Mapped[Company] = relationship(foreign_keys=[source_agency_id])
    destination_agency: Mapped[Company] = relationship(foreign_keys=[destination_agency_id])
    source_account: Mapped[Account] = relationship(foreign_keys=[source_account_id])
    destination_account: Mapped[Account] = relationship(foreign_keys=[destination_account_id])


class InterAgencyTransfer(Base):
    __tablename__ = "inter_agency_transfers"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    transfer_rule_id: Mapped[int] = mapped_column(ForeignKey("agency_transfer_rules.id"))
    source_agency_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    source_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    destination_agency_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    destination_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    note: Mapped[str | None] = mapped_column(Text)
    status: Mapped[InterAgencyTransferStatus] = mapped_column(SqlEnum(InterAgencyTransferStatus, name="inter_agency_transfer_status", values_callable=lambda x: [e.value for e in x]), default=InterAgencyTransferStatus.pending_receiver)
    created_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    receiver_decision_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    account_transfer_id: Mapped[int | None] = mapped_column(ForeignKey("account_transfers.id"))
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    decided_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    rule: Mapped[AgencyTransferRule] = relationship()
    source_agency: Mapped[Company] = relationship(foreign_keys=[source_agency_id])
    destination_agency: Mapped[Company] = relationship(foreign_keys=[destination_agency_id])
    source_account: Mapped[Account] = relationship(foreign_keys=[source_account_id])
    destination_account: Mapped[Account] = relationship(foreign_keys=[destination_account_id])


class InterAgencySettlement(Base):
    __tablename__ = "inter_agency_settlements"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    inter_agency_transfer_id: Mapped[int] = mapped_column(ForeignKey("inter_agency_transfers.id"))
    payer_agency_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    payer_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    receiver_agency_id: Mapped[int] = mapped_column(ForeignKey("companies.id"))
    receiver_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    debt_account_id: Mapped[int] = mapped_column(ForeignKey("accounts.id"))
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    note: Mapped[str | None] = mapped_column(Text)
    status: Mapped[str] = mapped_column(Text, default="pending")
    account_transfer_id: Mapped[int | None] = mapped_column(ForeignKey("account_transfers.id"))
    created_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    accepted_by_user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    accepted_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))
    inter_agency_transfer: Mapped[InterAgencyTransfer] = relationship()
    payer_agency: Mapped[Company] = relationship(foreign_keys=[payer_agency_id])
    receiver_agency: Mapped[Company] = relationship(foreign_keys=[receiver_agency_id])
    payer_account: Mapped[Account] = relationship(foreign_keys=[payer_account_id])
    receiver_account: Mapped[Account] = relationship(foreign_keys=[receiver_account_id])
    debt_account: Mapped[Account] = relationship(foreign_keys=[debt_account_id])
    account_transfer: Mapped[AccountTransfer | None] = relationship()


class Service(Base):
    __tablename__ = "services"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    company_id: Mapped[int | None] = mapped_column(ForeignKey("companies.id"))
    name: Mapped[str] = mapped_column(Text, unique=True)
    image_url: Mapped[str | None] = mapped_column(Text)
    transaction_type: Mapped[str | None] = mapped_column(Text)
    switch_type: Mapped[str | None] = mapped_column(Text)
    primary_account_id: Mapped[int | None] = mapped_column(ForeignKey("accounts.id"))
    secondary_account_id: Mapped[int | None] = mapped_column(ForeignKey("accounts.id"))
    routing_config: Mapped[dict | None] = mapped_column(JSONB)
    active: Mapped[bool] = mapped_column(Boolean, default=True)
    fee_rules: Mapped[list["ServiceFeeRule"]] = relationship(cascade="all, delete-orphan")


class ServiceFeeRule(Base):
    __tablename__ = "service_fee_rules"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    service_id: Mapped[int] = mapped_column(ForeignKey("services.id"))
    service_type: Mapped[Direction] = mapped_column(SqlEnum(Direction, name="transaction_direction", values_callable=lambda x: [e.value for e in x]))
    min_amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    max_amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    fee: Mapped[Decimal] = mapped_column(Numeric(14, 2))


class ServiceTransaction(Base):
    __tablename__ = "service_transactions"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    service_id: Mapped[int] = mapped_column(ForeignKey("services.id"))
    direction: Mapped[Direction] = mapped_column(SqlEnum(Direction, name="transaction_direction", values_callable=lambda x: [e.value for e in x]))
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    fee: Mapped[Decimal] = mapped_column(Numeric(14, 2), default=0)
    solde: Mapped[Decimal | None] = mapped_column(Numeric(14, 2))
    description: Mapped[str | None] = mapped_column(Text)
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    import_batch_id: Mapped[str | None] = mapped_column(Text)
    import_source: Mapped[str | None] = mapped_column(Text)
    reversed_at: Mapped[datetime | None] = mapped_column(DateTime(timezone=True))


class CashCount(Base):
    __tablename__ = "cash_counts"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    counted_on: Mapped[date] = mapped_column(Date)
    denomination: Mapped[Decimal] = mapped_column(Numeric(10, 2))
    quantity: Mapped[int] = mapped_column(Integer)
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))


class AgencyLedgerEntry(Base):
    __tablename__ = "agency_ledger_entries"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    kind: Mapped[str] = mapped_column(Text)
    category: Mapped[str] = mapped_column(Text)
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    description: Mapped[str | None] = mapped_column(Text)
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    locked: Mapped[bool] = mapped_column(Boolean, default=False, server_default="false")


class UnpaidItem(Base):
    __tablename__ = "unpaid_items"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    person_name: Mapped[str] = mapped_column(Text)
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    description: Mapped[str | None] = mapped_column(Text)
    settled: Mapped[bool] = mapped_column(Boolean, default=False)


class UnpaidMovement(Base):
    __tablename__ = "unpaid_movements"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    person_name: Mapped[str] = mapped_column(Text)
    direction: Mapped[str] = mapped_column(Text)
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    description: Mapped[str | None] = mapped_column(Text)
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))


class SalafEntry(Base):
    __tablename__ = "salaf_entries"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    investor: Mapped[str] = mapped_column(Text)
    direction: Mapped[str] = mapped_column(Text)
    amount: Mapped[Decimal] = mapped_column(Numeric(14, 2))
    occurred_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))


class AuditLog(Base):
    __tablename__ = "audit_logs"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    user_id: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    user_name: Mapped[str | None] = mapped_column(Text)
    area: Mapped[AuditArea] = mapped_column(SqlEnum(AuditArea, name="audit_area", values_callable=lambda x: [e.value for e in x]))
    action: Mapped[str] = mapped_column(Text)
    title: Mapped[str | None] = mapped_column(Text)
    details: Mapped[str | None] = mapped_column(Text)
    extra: Mapped[dict] = mapped_column("metadata", JSONB, default=dict)
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True), server_default=func.now())


class RegisterTable(Base):
    __tablename__ = "register_tables"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    name: Mapped[str] = mapped_column(Text, unique=True)
    description: Mapped[str | None] = mapped_column(Text)
    created_by: Mapped[int | None] = mapped_column(ForeignKey("users.id"))
    columns: Mapped[list["RegisterColumn"]] = relationship(cascade="all, delete-orphan")


class RegisterColumn(Base):
    __tablename__ = "register_columns"

    id: Mapped[int] = mapped_column(BigInteger, primary_key=True)
    table_id: Mapped[int] = mapped_column(ForeignKey("register_tables.id"))
    key: Mapped[str] = mapped_column(Text)
    header: Mapped[str] = mapped_column(Text)
    position: Mapped[int] = mapped_column(Integer, default=0)
