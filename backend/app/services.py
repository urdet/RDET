from datetime import date, datetime, timezone
from decimal import Decimal

from fastapi import HTTPException
from sqlalchemy import func, select, text
from sqlalchemy.orm import Session

from app.models import (
    Account,
    AccountLedgerEntry,
    AccountTransfer,
    AgencyTransferRule,
    AuditArea,
    AuditLog,
    CashCount,
    Company,
    Direction,
    InterAgencyTransfer,
    InterAgencyTransferStatus,
    SalafEntry,
    Service,
    ServiceFeeRule,
    ServiceTransaction,
    UnpaidItem,
    User,
)


def audit(db: Session, user: User | None, area: AuditArea, action: str, title: str, details: str = "", **metadata: object) -> None:
    db.add(
        AuditLog(
            user_id=user.id if user else None,
            user_name=f"{user.first_name} {user.last_name}" if user else None,
            area=area,
            action=action,
            title=title,
            details=details,
            extra=metadata,
        )
    )


def get_account(db: Session, account_id: int | None) -> Account | None:
    if account_id is None:
        return None
    account = db.get(Account, account_id)
    if not account:
        raise HTTPException(status_code=404, detail=f"Account {account_id} not found")
    return account


def account_normal_side(account: Account) -> str:
    return account.normal_balance_side if account.normal_balance_side in {"debit", "credit"} else "debit"


def account_display_balance(account: Account) -> Decimal:
    debit = Decimal(account.debit_total or 0)
    credit = Decimal(account.credit_total or 0)
    return debit - credit if account_normal_side(account) == "debit" else credit - debit


def sync_account_balance(account: Account) -> None:
    account.balance = account_display_balance(account)


def reset_account_opening_balance(account: Account, balance: Decimal, normal_side: str | None = None) -> None:
    if normal_side in {"debit", "credit"}:
        account.normal_balance_side = normal_side
    side = account_normal_side(account)
    amount = Decimal(balance)
    if side == "debit":
        account.debit_total = amount if amount >= 0 else Decimal("0")
        account.credit_total = -amount if amount < 0 else Decimal("0")
    else:
        account.credit_total = amount if amount >= 0 else Decimal("0")
        account.debit_total = -amount if amount < 0 else Decimal("0")
    sync_account_balance(account)


def ledger_side_for_delta(account: Account, delta: Decimal) -> str:
    if delta >= 0:
        return "debit" if account_normal_side(account) == "debit" else "credit"
    return "credit" if account_normal_side(account) == "debit" else "debit"


def account_action_rules(db: Session, company_id: int | None) -> list[dict]:
    if not company_id:
        return []
    value = db.execute(
        text("SELECT value FROM agency_settings WHERE company_id = :company_id AND key = 'account_action_rules'"),
        {"company_id": company_id},
    ).scalar()
    if not isinstance(value, dict):
        return []
    rules = value.get("rules")
    return rules if isinstance(rules, list) else []


def apply_balance_delta(
    db: Session,
    account: Account,
    amount: Decimal,
    *,
    account_transfer_id: int | None = None,
    description: str | None = None,
    occurred_at: datetime | None = None,
    created_by: int | None = None,
    record_ledger: bool = True,
) -> None:
    sync_account_balance(account)
    account.previous_balance = account.balance
    delta = Decimal(amount)
    side = account_normal_side(account)
    if delta >= 0:
        if side == "debit":
            account.debit_total = Decimal(account.debit_total or 0) + delta
        else:
            account.credit_total = Decimal(account.credit_total or 0) + delta
    else:
        if side == "debit":
            account.credit_total = Decimal(account.credit_total or 0) + abs(delta)
        else:
            account.debit_total = Decimal(account.debit_total or 0) + abs(delta)
    sync_account_balance(account)
    account.updated_at = datetime.now(timezone.utc)
    if record_ledger and delta != 0:
        db.add(
            AccountLedgerEntry(
                account_id=account.id,
                account_transfer_id=account_transfer_id,
                side=ledger_side_for_delta(account, delta),
                amount=abs(delta),
                balance_effect=delta,
                balance_after=account.balance,
                description=description,
                occurred_at=occurred_at or datetime.now(timezone.utc),
                created_by=created_by,
            )
        )


def apply_account_actions(
    db: Session,
    user: User,
    trigger_account_id: int | None,
    event: str,
    amount: Decimal,
    *,
    account_transfer_id: int | None = None,
    occurred_at: datetime | None = None,
) -> None:
    if trigger_account_id is None:
        return

    for rule in account_action_rules(db, user.company_id):
        if not rule.get("enabled", True):
            continue
        if str(rule.get("accountId") or "") != str(trigger_account_id):
            continue
        if rule.get("event") != event:
            continue

        effect = rule.get("effect") if rule.get("effect") in {"add", "subtract"} else ("add" if event == "money_in" else "subtract")
        delta = amount if effect == "add" else -amount
        linked_account_ids = rule.get("linkedAccountIds") if isinstance(rule.get("linkedAccountIds"), list) else []

        for linked_account_id in linked_account_ids:
            try:
                account_id = int(linked_account_id)
            except (TypeError, ValueError):
                continue
            account = get_account(db, account_id)
            if not account:
                continue
            if user.company_id and user.company_id not in account.company_ids:
                raise HTTPException(status_code=403, detail="Linked account does not belong to active agency")
            apply_balance_delta(
                db,
                account,
                delta,
                account_transfer_id=account_transfer_id,
                description=f"Account action: {rule.get('name') or event}",
                occurred_at=occurred_at,
                created_by=user.id,
            )
            audit(
                db,
                user,
                AuditArea.account,
                "action",
                "Account action",
                rule=rule.get("name") or "",
                event=event,
                trigger_account_id=trigger_account_id,
                linked_account_id=account.id,
                amount=str(amount),
                effect=effect,
            )


def create_transfer(db: Session, payload, user: User, commit: bool = True) -> AccountTransfer:
    if not payload.from_account_id and not payload.to_account_id:
        raise HTTPException(status_code=400, detail="At least one side of the transfer is required")
    source = get_account(db, payload.from_account_id)
    target = get_account(db, payload.to_account_id)
    amount = Decimal(payload.amount)
    occurred_at = payload.occurred_at or datetime.now(timezone.utc)
    transfer = AccountTransfer(
        from_account_id=payload.from_account_id,
        to_account_id=payload.to_account_id,
        amount=amount,
        description=payload.description,
        contributions=getattr(payload, "contributions", None),
        occurred_at=occurred_at,
        created_by=user.id,
    )
    db.add(transfer)
    db.flush()
    if source:
        apply_balance_delta(db, source, -amount, account_transfer_id=transfer.id, description=payload.description, occurred_at=occurred_at, created_by=user.id)
    if target:
        apply_balance_delta(db, target, amount, account_transfer_id=transfer.id, description=payload.description, occurred_at=occurred_at, created_by=user.id)
    apply_account_actions(db, user, payload.from_account_id, "money_out", amount, account_transfer_id=transfer.id, occurred_at=occurred_at)
    apply_account_actions(db, user, payload.to_account_id, "money_in", amount, account_transfer_id=transfer.id, occurred_at=occurred_at)
    audit(db, user, AuditArea.transaction, "create", "Account transfer", amount=str(amount), from_account_id=payload.from_account_id, to_account_id=payload.to_account_id)
    if commit:
        db.commit()
        db.refresh(transfer)
    return transfer


def accept_inter_agency_transfer(db: Session, transfer: InterAgencyTransfer, user: User) -> InterAgencyTransfer:
    if transfer.status != InterAgencyTransferStatus.pending_receiver:
        raise HTTPException(status_code=409, detail="Transfer is not pending")
    if user.company_id != transfer.destination_agency_id:
        raise HTTPException(status_code=403, detail="Only the receiver agency can accept this transfer")

    locked_transfer = db.scalar(
        select(InterAgencyTransfer)
        .where(InterAgencyTransfer.id == transfer.id)
        .with_for_update()
    )
    if not locked_transfer or locked_transfer.status != InterAgencyTransferStatus.pending_receiver:
        raise HTTPException(status_code=409, detail="Transfer is no longer pending")

    source = db.scalar(select(Account).where(Account.id == locked_transfer.source_account_id).with_for_update())
    target = db.scalar(select(Account).where(Account.id == locked_transfer.destination_account_id).with_for_update())
    if not source or locked_transfer.source_agency_id not in source.company_ids:
        raise HTTPException(status_code=404, detail="Source account not found")
    if not target or locked_transfer.destination_agency_id not in target.company_ids:
        raise HTTPException(status_code=404, detail="Destination account not found")

    amount = Decimal(locked_transfer.amount)
    source_agency_name = db.scalar(select(Company.name).where(Company.id == locked_transfer.source_agency_id)) or "Source agency"
    destination_agency_name = db.scalar(select(Company.name).where(Company.id == locked_transfer.destination_agency_id)) or "Destination agency"

    account_transfer = AccountTransfer(
        from_account_id=source.id,
        to_account_id=target.id,
        amount=amount,
        description=locked_transfer.note or "Inter-agency transfer",
        contributions=[
            {
                "account_id": source.id,
                "agency_id": locked_transfer.destination_agency_id,
                "name": destination_agency_name,
                "amount": str(amount),
                "direction": "versement",
            },
            {
                "account_id": target.id,
                "agency_id": locked_transfer.source_agency_id,
                "name": source_agency_name,
                "amount": str(amount),
                "direction": "versement",
            },
        ],
        occurred_at=datetime.now(timezone.utc),
        created_by=locked_transfer.created_by_user_id,
    )
    db.add(account_transfer)
    db.flush()
    apply_balance_delta(db, source, amount, account_transfer_id=account_transfer.id, description=account_transfer.description, occurred_at=account_transfer.occurred_at, created_by=account_transfer.created_by)
    apply_balance_delta(db, target, amount, account_transfer_id=account_transfer.id, description=account_transfer.description, occurred_at=account_transfer.occurred_at, created_by=account_transfer.created_by)

    locked_transfer.status = InterAgencyTransferStatus.accepted
    locked_transfer.receiver_decision_by_user_id = user.id
    locked_transfer.decided_at = datetime.now(timezone.utc)
    locked_transfer.account_transfer_id = account_transfer.id
    audit(
        db,
        user,
        AuditArea.transaction,
        "transfer_accepted",
        "Inter-agency transfer accepted",
        transfer_id=locked_transfer.id,
        amount=str(amount),
        source_agency_id=locked_transfer.source_agency_id,
        destination_agency_id=locked_transfer.destination_agency_id,
    )
    db.commit()
    db.refresh(locked_transfer)
    return locked_transfer


def fee_for(db: Session, service_id: int, direction: Direction, amount: Decimal) -> Decimal:
    rule = db.scalar(
        select(ServiceFeeRule).where(
            ServiceFeeRule.service_id == service_id,
            ServiceFeeRule.service_type == direction,
            ServiceFeeRule.min_amount <= amount,
            ServiceFeeRule.max_amount >= amount,
        )
    )
    return Decimal(rule.fee) if rule else Decimal("0")


def account_by_names(db: Session, company_id: int | None, names: list[str]) -> Account:
    normalized_names = [name.lower() for name in names]
    query = select(Account).where(func.lower(Account.name).in_(normalized_names))
    if company_id:
        query = query.where(Account.company_ids.any(company_id))
    account = db.scalar(query.order_by(Account.legacy_id.nullslast(), Account.name))
    if not account:
        raise HTTPException(status_code=400, detail=f"Required account not found: {names[0]}")
    return account


def service_accounts(service: Service, direction: Direction) -> tuple[int, int]:
    if not service.primary_account_id or not service.secondary_account_id:
        raise HTTPException(status_code=400, detail="Service account routing is not configured")
    if service.switch_type and service.switch_type == direction.value:
        return service.primary_account_id, service.secondary_account_id
    if service.switch_type:
        return service.secondary_account_id, service.primary_account_id
    return service.primary_account_id, service.secondary_account_id


def service_cash_accounts(db: Session, user: User, direction: Direction) -> tuple[int, int]:
    cash = account_by_names(db, user.company_id, ["Caisse Calculee", "Caisse Calculée", "Caise Calcule", "Caise Calculee"])
    fundex = account_by_names(db, user.company_id, ["Fundex"])
    if direction == Direction.in_:
        return cash.id, fundex.id
    return fundex.id, cash.id


def service_configured_accounts(db: Session, user: User, service: Service, direction: Direction) -> tuple[int, int] | None:
    routing_config = service.routing_config if isinstance(service.routing_config, dict) else {}
    route = routing_config.get(direction.value)
    if not isinstance(route, dict):
        return None
    try:
        from_id = int(route.get("from_account_id") or 0)
        to_id = int(route.get("to_account_id") or 0)
    except (TypeError, ValueError):
        return None
    if not from_id or not to_id:
        return None
    for account_id in [from_id, to_id]:
        account = get_account(db, account_id)
        if not account or (user.company_id and user.company_id not in account.company_ids):
            raise HTTPException(status_code=403, detail="Configured service route account does not belong to active agency")
    return from_id, to_id


def service_route_accounts(db: Session, user: User, service: Service, direction: Direction) -> tuple[int, int]:
    configured_accounts = service_configured_accounts(db, user, service, direction)
    if configured_accounts:
        return configured_accounts
    if service.primary_account_id and service.secondary_account_id:
        return service_accounts(service, direction)
    return service_cash_accounts(db, user, direction)


def service_supports_direction(service: Service, direction: Direction) -> bool:
    service_type = (service.transaction_type or service.switch_type or "IN & OUT").upper()
    if service_type in {"IN & OUT", "[IN][OUT]", ""}:
        return True
    return direction.value in service_type


def existing_service_transaction_by_solde(db: Session, user: User, solde: Decimal | None) -> ServiceTransaction | None:
    if solde is None:
        return None
    query = select(ServiceTransaction).where(
        ServiceTransaction.solde == solde,
        ServiceTransaction.reversed_at.is_(None),
    )
    if user.company_id:
        query = query.join(User, User.id == ServiceTransaction.created_by).where(User.company_id == user.company_id)
    return db.scalar(query.order_by(ServiceTransaction.id.desc()).limit(1))


def record_service_transfer_delta(db: Session, user: User, service: Service, direction: Direction, amount_delta: Decimal, occurred_at: datetime | None, description: str) -> None:
    if amount_delta == 0:
        return
    from_id, to_id = service_route_accounts(db, user, service, direction)
    amount = abs(amount_delta)
    if amount_delta < 0:
        from_id, to_id = to_id, from_id
    create_transfer(
        db,
        type("TransferPayload", (), {
            "from_account_id": from_id,
            "to_account_id": to_id,
            "amount": amount,
            "description": description,
            "occurred_at": occurred_at,
        })(),
        user,
        commit=False,
    )


def payload_fee(db: Session, service: Service, direction: Direction, amount: Decimal, payload) -> Decimal:
    manual_fee = getattr(payload, "fee", None)
    return Decimal(manual_fee or 0) if manual_fee is not None else fee_for(db, service.id, direction, amount)


def transaction_effect_amount(amount: Decimal, fee: Decimal) -> Decimal:
    return amount + fee


def update_service_transaction(db: Session, tx: ServiceTransaction, amount: Decimal, fee: Decimal | None, user: User) -> ServiceTransaction:
    service = db.get(Service, tx.service_id)
    if not service:
        raise HTTPException(status_code=404, detail="Service not found")
    new_amount = Decimal(amount)
    new_fee = Decimal(fee or 0) if fee is not None else fee_for(db, service.id, tx.direction, new_amount)
    old_amount = Decimal(tx.amount)
    old_fee = Decimal(tx.fee or 0)
    record_service_transfer_delta(
        db,
        user,
        service,
        tx.direction,
        transaction_effect_amount(new_amount, new_fee) - transaction_effect_amount(old_amount, old_fee),
        tx.occurred_at,
        f"Correction {service.name} {tx.direction.value}",
    )
    tx.amount = new_amount
    tx.fee = new_fee
    audit(db, user, AuditArea.service, "update", "Service transaction correction", service=service.name, transaction_id=tx.id, old_amount=str(old_amount), old_fee=str(old_fee), amount=str(new_amount), fee=str(new_fee))
    db.commit()
    db.refresh(tx)
    return tx


def reverse_service_transaction(db: Session, tx: ServiceTransaction, user: User) -> ServiceTransaction:
    service = db.get(Service, tx.service_id)
    if not service:
        raise HTTPException(status_code=404, detail="Service not found")
    record_service_transfer_delta(
        db,
        user,
        service,
        tx.direction,
        -transaction_effect_amount(Decimal(tx.amount), Decimal(tx.fee or 0)),
        tx.occurred_at,
        f"Annulation {service.name} {tx.direction.value}",
    )
    tx.reversed_at = datetime.now(timezone.utc)
    audit(db, user, AuditArea.service, "delete", "Service transaction reversed", service=service.name, transaction_id=tx.id, amount=str(tx.amount))
    db.commit()
    db.refresh(tx)
    return tx


def create_service_transaction(db: Session, payload, user: User) -> ServiceTransaction:
    service = db.get(Service, payload.service_id)
    if not service:
        raise HTTPException(status_code=404, detail="Service not found")
    if not service.active:
        raise HTTPException(status_code=400, detail="Service is inactive")
    if not service_supports_direction(service, payload.direction):
        raise HTTPException(status_code=400, detail=f"{service.name} does not support {payload.direction.value}")
    amount = Decimal(payload.amount)
    solde = Decimal(payload.solde) if getattr(payload, "solde", None) is not None else None
    existing = existing_service_transaction_by_solde(db, user, solde)
    if existing:
        raise HTTPException(status_code=409, detail="Duplicate solde")
    fee = payload_fee(db, service, payload.direction, amount, payload)
    record_service_transfer_delta(db, user, service, payload.direction, transaction_effect_amount(amount, fee), payload.occurred_at, f"{service.name} {payload.direction.value}")
    tx = ServiceTransaction(
        service_id=service.id,
        direction=payload.direction,
        amount=amount,
        fee=fee,
        solde=solde,
        description=payload.description,
        occurred_at=payload.occurred_at or datetime.now(timezone.utc),
        created_by=user.id,
    )
    db.add(tx)
    audit(db, user, AuditArea.service, "create", "Service transaction", service=service.name, amount=str(amount), fee=str(fee))
    db.commit()
    db.refresh(tx)
    return tx


def save_cash_count(db: Session, payload, user: User) -> dict[str, Decimal]:
    db.query(CashCount).filter(CashCount.counted_on == payload.counted_on).delete()
    total = Decimal("0")
    for denomination, quantity in payload.counts.items():
        total += Decimal(denomination) * quantity
        db.add(CashCount(counted_on=payload.counted_on, denomination=denomination, quantity=quantity, created_by=user.id))
    settings = db.execute(
        text("SELECT value FROM agency_settings WHERE company_id = :company_id AND key = 'app_settings'"),
        {"company_id": user.company_id},
    ).scalar() if user.company_id else {}
    cash_account_id = settings.get("cashAccountId") if isinstance(settings, dict) else None
    if cash_account_id:
        try:
            account = db.get(Account, int(cash_account_id))
        except (TypeError, ValueError):
            account = None
        if account and (not user.company_id or user.company_id in account.company_ids):
            apply_balance_delta(db, account, total - account_display_balance(account), description="Cash count adjustment", created_by=user.id)
            account.updated_at = datetime.now(timezone.utc)
    audit(db, user, AuditArea.cash, "save", "Cash count", counted_on=str(payload.counted_on), total=str(total))
    db.commit()
    return {"total": total}


def dashboard_summary(db: Session, company_id: int | None = None, hidden_account_ids: set[int] | None = None) -> dict:
    today = date.today()
    accounts_query = select(Account)
    if company_id:
        accounts_query = accounts_query.where(Account.company_ids.any(company_id))
    accounts = db.scalars(accounts_query.order_by(Account.legacy_id.nullslast(), Account.name)).all()
    if hidden_account_ids:
        accounts = [account for account in accounts if account.id not in hidden_account_ids]
    for account in accounts:
        sync_account_balance(account)
    total_balance = sum((Decimal(a.balance) for a in accounts), Decimal("0"))
    total_debit = sum((Decimal(a.debit_total or 0) for a in accounts), Decimal("0"))
    total_credit = sum((Decimal(a.credit_total or 0) for a in accounts), Decimal("0"))
    service_in_query = select(func.coalesce(func.sum(ServiceTransaction.amount), 0)).where(ServiceTransaction.direction == Direction.in_, func.date(ServiceTransaction.occurred_at) == today)
    service_out_query = select(func.coalesce(func.sum(ServiceTransaction.amount), 0)).where(ServiceTransaction.direction == Direction.out, func.date(ServiceTransaction.occurred_at) == today)
    fees_query = select(func.coalesce(func.sum(ServiceTransaction.fee), 0)).where(func.date(ServiceTransaction.occurred_at) == today)
    if company_id:
        service_in_query = service_in_query.join(User, User.id == ServiceTransaction.created_by).where(User.company_id == company_id)
        service_out_query = service_out_query.join(User, User.id == ServiceTransaction.created_by).where(User.company_id == company_id)
        fees_query = fees_query.join(User, User.id == ServiceTransaction.created_by).where(User.company_id == company_id)
    service_in = db.scalar(service_in_query)
    service_out = db.scalar(service_out_query)
    fees = db.scalar(fees_query)
    unpaid_total = db.scalar(select(func.coalesce(func.sum(UnpaidItem.amount), 0)).where(UnpaidItem.settled.is_(False)))
    cash_real = db.scalar(
        select(func.coalesce(func.sum(CashCount.denomination * CashCount.quantity), 0)).where(CashCount.counted_on == today)
    )
    return {
        "total_balance": total_balance,
        "total_debit": total_debit,
        "total_credit": total_credit,
        "service_in": service_in,
        "service_out": service_out,
        "fees": fees,
        "unpaid_total": unpaid_total,
        "cash_real": cash_real,
        "credit": service_in,
        "debit": service_out,
        "total_sales": service_in,
        "total_purchases": service_out,
        "accounts": accounts,
    }


def create_salaf(db: Session, payload, user: User) -> SalafEntry:
    entry = SalafEntry(investor=payload.investor, direction=payload.direction, amount=payload.amount, created_by=user.id)
    db.add(entry)
    audit(db, user, AuditArea.transaction, "create", "Salaf entry", investor=payload.investor, direction=payload.direction, amount=str(payload.amount))
    db.commit()
    db.refresh(entry)
    return entry
