from datetime import date, datetime
from decimal import Decimal
from typing import Any

from pydantic import BaseModel, Field

from app.models import AgencyLinkStatus, AgencyTransferRuleStatus, Direction, InterAgencyTransferStatus, UserRole


class Token(BaseModel):
    access_token: str
    token_type: str = "bearer"


class CompanyOut(BaseModel):
    id: int
    name: str

    model_config = {"from_attributes": True}


class UserOut(BaseModel):
    id: int
    company_id: int | None
    username: str
    first_name: str
    last_name: str
    email: str | None = None
    phone: str | None = None
    image_url: str | None = None
    role: UserRole
    permissions: dict[str, Any] | None = None
    active: bool
    company: CompanyOut | None

    model_config = {"from_attributes": True}


class UserCreate(BaseModel):
    first_name: str = Field(min_length=1, max_length=80)
    last_name: str = Field(min_length=1, max_length=80)
    username: str = Field(min_length=3, max_length=80)
    password: str = Field(min_length=6, max_length=128)
    role: UserRole = UserRole.user
    permissions: dict[str, Any] | None = None
    active: bool = True


class UserUpdate(BaseModel):
    first_name: str = Field(min_length=1, max_length=80)
    last_name: str = Field(min_length=1, max_length=80)
    username: str = Field(min_length=3, max_length=80)
    role: UserRole
    permissions: dict[str, Any] | None = None
    active: bool = True
    password: str | None = Field(default=None, min_length=6, max_length=128)


class LoginIn(BaseModel):
    username: str
    password: str


class RegisterIn(BaseModel):
    agency_name: str = Field(min_length=2, max_length=120)
    first_name: str = Field(min_length=1, max_length=80)
    last_name: str = Field(min_length=1, max_length=80)
    username: str = Field(min_length=3, max_length=80)
    password: str = Field(min_length=6, max_length=128)


class AgencyCreate(BaseModel):
    name: str = Field(min_length=2, max_length=120)


class ProfileUpdate(BaseModel):
    first_name: str = Field(min_length=1, max_length=80)
    last_name: str = Field(min_length=1, max_length=80)
    email: str | None = Field(default=None, max_length=160)
    phone: str | None = Field(default=None, max_length=60)
    image_url: str | None = Field(default=None, max_length=3_000_000)


class CredentialsUpdate(BaseModel):
    username: str = Field(min_length=3, max_length=80)
    current_password: str | None = Field(default=None, max_length=128)
    new_password: str | None = Field(default=None, min_length=6, max_length=128)


class AccountOut(BaseModel):
    id: int
    name: str
    balance: Decimal
    previous_balance: Decimal | None = None
    debit_total: Decimal = 0
    credit_total: Decimal = 0
    normal_balance_side: str = "debit"
    visible: bool
    company_ids: list[int]
    legacy_id: int | None
    updated_at: datetime

    model_config = {"from_attributes": True}


class AccountCreate(BaseModel):
    name: str
    balance: Decimal = 0
    visible: bool = True
    normal_balance_side: str = Field(default="debit", pattern=r"^(debit|credit)$")


class AccountBalanceUpdate(BaseModel):
    balance: Decimal
    normal_balance_side: str | None = Field(default=None, pattern=r"^(debit|credit)$")


class AccountsScreenSettingsIn(BaseModel):
    config: dict[str, Any]


class AppConfigImportIn(BaseModel):
    config: dict[str, Any]


class TransferCreate(BaseModel):
    from_account_id: int | None = None
    to_account_id: int | None = None
    amount: Decimal = Field(gt=0)
    description: str | None = None
    occurred_at: datetime | None = None
    context_account_id: int | None = None
    contributions: list[dict[str, Any]] | None = None


class AgencyLinkCreate(BaseModel):
    agency_id: int


class AgencyLinkOut(BaseModel):
    id: int
    agency_a_id: int
    agency_b_id: int
    agency_a_name: str | None = None
    agency_b_name: str | None = None
    status: AgencyLinkStatus
    requested_agency_id: int | None = None
    target_agency_id: int | None = None
    requested_by_user_id: int | None = None
    accepted_by_user_id: int | None = None
    created_at: datetime
    accepted_at: datetime | None = None
    disabled_at: datetime | None = None


class AgencyTransferRuleCreate(BaseModel):
    agency_link_id: int
    source_agency_id: int
    source_account_id: int
    destination_agency_id: int
    destination_account_id: int
    name: str = Field(min_length=1, max_length=160)
    description: str | None = None


class AgencyTransferRuleOut(BaseModel):
    id: int
    agency_link_id: int
    source_agency_id: int
    source_account_id: int
    destination_agency_id: int
    destination_account_id: int
    source_agency_name: str | None = None
    source_account_name: str | None = None
    destination_agency_name: str | None = None
    destination_account_name: str | None = None
    name: str
    description: str | None = None
    status: AgencyTransferRuleStatus
    active: bool
    created_by_user_id: int | None = None
    accepted_by_user_id: int | None = None
    created_at: datetime
    accepted_at: datetime | None = None


class InterAgencyTransferCreate(BaseModel):
    transfer_rule_id: int
    amount: Decimal = Field(gt=0)
    note: str | None = None


class InterAgencyTransferOut(BaseModel):
    id: int
    transfer_rule_id: int
    source_agency_id: int
    source_account_id: int
    destination_agency_id: int
    destination_account_id: int
    source_agency_name: str | None = None
    source_account_name: str | None = None
    destination_agency_name: str | None = None
    destination_account_name: str | None = None
    rule_name: str | None = None
    amount: Decimal
    note: str | None = None
    status: InterAgencyTransferStatus
    created_by_user_id: int | None = None
    receiver_decision_by_user_id: int | None = None
    created_at: datetime
    decided_at: datetime | None = None
    settled_amount: Decimal = 0
    remaining_amount: Decimal = 0


class InterAgencySettlementCreate(BaseModel):
    inter_agency_transfer_id: int
    payer_account_id: int
    receiver_account_id: int
    amount: Decimal = Field(gt=0)
    note: str | None = None


class InterAgencySettlementOut(BaseModel):
    id: int
    inter_agency_transfer_id: int
    payer_agency_id: int
    payer_account_id: int
    payer_agency_name: str | None = None
    payer_account_name: str | None = None
    receiver_agency_id: int
    receiver_account_id: int
    receiver_agency_name: str | None = None
    receiver_account_name: str | None = None
    debt_account_id: int
    debt_account_name: str | None = None
    amount: Decimal
    note: str | None = None
    status: str = "pending"
    account_transfer_id: int | None = None
    created_by_user_id: int | None = None
    accepted_by_user_id: int | None = None
    created_at: datetime
    accepted_at: datetime | None = None


class ServiceOut(BaseModel):
    id: int
    company_id: int | None = None
    name: str
    image_url: str | None
    transaction_type: str | None
    switch_type: str | None
    primary_account_id: int | None
    secondary_account_id: int | None
    routing_config: dict[str, Any] | None = None
    active: bool

    model_config = {"from_attributes": True}


class ServiceCreate(BaseModel):
    name: str
    image_url: str | None = Field(default=None, max_length=3_000_000)
    transaction_type: str = Field(pattern=r"^(IN|OUT|IN & OUT)$")
    switch_type: str | None = None
    primary_account_id: int | None = None
    secondary_account_id: int | None = None
    routing_config: dict[str, Any] | None = None
    active: bool = True


class ServiceUpdate(BaseModel):
    name: str
    image_url: str | None = Field(default=None, max_length=3_000_000)
    transaction_type: str = Field(pattern=r"^(IN|OUT|IN & OUT)$")
    routing_config: dict[str, Any] | None = None
    active: bool = True


class FeeRuleCreate(BaseModel):
    service_id: int
    service_type: Direction
    min_amount: Decimal
    max_amount: Decimal
    fee: Decimal


class ServiceTransactionCreate(BaseModel):
    service_id: int
    direction: Direction
    amount: Decimal = Field(gt=0)
    fee: Decimal | None = Field(default=None, ge=0)
    solde: Decimal | None = None
    description: str | None = None
    occurred_at: datetime | None = None
    import_batch_id: str | None = Field(default=None, max_length=120)
    import_source: str | None = Field(default=None, max_length=120)


class ServiceTransactionUpdate(BaseModel):
    amount: Decimal = Field(gt=0)
    fee: Decimal | None = Field(default=None, ge=0)


class CashCountIn(BaseModel):
    counted_on: date
    counts: dict[Decimal, int]


class AgencyLedgerEntryIn(BaseModel):
    kind: str = Field(pattern=r"^(expense|income)$")
    category: str = Field(min_length=1, max_length=120)
    amount: Decimal = Field(gt=0)
    description: str | None = None
    occurred_at: datetime | None = None


class AgencyLedgerEntryOut(AgencyLedgerEntryIn):
    id: int
    created_by: int | None = None

    model_config = {"from_attributes": True}


class UnpaidItemIn(BaseModel):
    person_name: str
    amount: Decimal
    description: str | None = None


class UnpaidItemOut(UnpaidItemIn):
    id: int
    settled: bool

    model_config = {"from_attributes": True}


class UnpaidMovementIn(BaseModel):
    person_name: str = Field(min_length=1, max_length=160)
    direction: str = Field(pattern=r"^\+|-$")
    amount: Decimal = Field(gt=0)
    description: str | None = None


class SalafEntryIn(BaseModel):
    investor: str
    direction: str = Field(pattern=r"^\+|-$")
    amount: Decimal = Field(gt=0)


class DashboardSummary(BaseModel):
    total_balance: Decimal
    total_debit: Decimal = 0
    total_credit: Decimal = 0
    service_in: Decimal
    service_out: Decimal
    fees: Decimal
    unpaid_total: Decimal
    cash_real: Decimal
    credit: Decimal
    debit: Decimal
    total_sales: Decimal
    total_purchases: Decimal
    accounts: list[AccountOut]
