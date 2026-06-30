CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE TYPE user_role AS ENUM ('Admin', 'Chef', 'User');
CREATE TYPE transaction_direction AS ENUM ('IN', 'OUT');
CREATE TYPE audit_area AS ENUM ('auth', 'user', 'company', 'account', 'service', 'transaction', 'cash', 'register', 'report');
CREATE TYPE agency_link_status AS ENUM ('pending', 'active', 'rejected', 'disabled');
CREATE TYPE agency_transfer_rule_status AS ENUM ('pending', 'active', 'rejected', 'disabled');
CREATE TYPE inter_agency_transfer_status AS ENUM ('pending_receiver', 'accepted', 'cancelled', 'rejected');

CREATE TABLE companies (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT,
    seo TEXT,
    chef TEXT,
    admin_local TEXT,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    company_id BIGINT REFERENCES companies(id),
    username TEXT NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    info TEXT,
    role user_role NOT NULL DEFAULT 'User',
    image_url TEXT,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE accounts (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    balance NUMERIC(14,2) NOT NULL DEFAULT 0 CHECK (balance >= -999999999999.99),
    visible BOOLEAN NOT NULL DEFAULT TRUE,
    company_ids BIGINT[] NOT NULL DEFAULT '{}',
    legacy_id INTEGER UNIQUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE account_transfers (
    id BIGSERIAL PRIMARY KEY,
    from_account_id BIGINT REFERENCES accounts(id),
    to_account_id BIGINT REFERENCES accounts(id),
    amount NUMERIC(14,2) NOT NULL CHECK (amount > 0),
    description TEXT,
    is_paid BOOLEAN NOT NULL DEFAULT FALSE,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    created_by BIGINT REFERENCES users(id),
    reversed_at TIMESTAMPTZ,
    reversal_reason TEXT
);

CREATE TABLE agency_links (
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
);

CREATE TABLE agency_transfer_rules (
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
);

CREATE TABLE inter_agency_transfers (
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
);

CREATE TABLE services (
    id BIGSERIAL PRIMARY KEY,
    company_id BIGINT REFERENCES companies(id),
    name TEXT NOT NULL UNIQUE,
    image_url TEXT,
    transaction_type TEXT,
    switch_type TEXT,
    primary_account_id BIGINT REFERENCES accounts(id),
    secondary_account_id BIGINT REFERENCES accounts(id),
    routing_config JSONB,
    active BOOLEAN NOT NULL DEFAULT TRUE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE service_fee_rules (
    id BIGSERIAL PRIMARY KEY,
    service_id BIGINT NOT NULL REFERENCES services(id) ON DELETE CASCADE,
    service_type transaction_direction NOT NULL,
    min_amount NUMERIC(14,2) NOT NULL,
    max_amount NUMERIC(14,2) NOT NULL,
    fee NUMERIC(14,2) NOT NULL DEFAULT 0,
    CHECK (min_amount <= max_amount)
);

CREATE TABLE service_transactions (
    id BIGSERIAL PRIMARY KEY,
    service_id BIGINT NOT NULL REFERENCES services(id),
    direction transaction_direction NOT NULL,
    amount NUMERIC(14,2) NOT NULL CHECK (amount > 0),
    fee NUMERIC(14,2) NOT NULL DEFAULT 0,
    description TEXT,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    created_by BIGINT REFERENCES users(id),
    reversed_at TIMESTAMPTZ,
    reversal_reason TEXT
);

CREATE TABLE account_balance_snapshots (
    id BIGSERIAL PRIMARY KEY,
    account_id BIGINT NOT NULL REFERENCES accounts(id),
    balance NUMERIC(14,2) NOT NULL,
    snapshot_date DATE NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    UNIQUE (account_id, snapshot_date)
);

CREATE TABLE cash_counts (
    id BIGSERIAL PRIMARY KEY,
    counted_on DATE NOT NULL,
    denomination NUMERIC(10,2) NOT NULL,
    quantity INTEGER NOT NULL CHECK (quantity >= 0),
    created_by BIGINT REFERENCES users(id),
    UNIQUE (counted_on, denomination)
);

CREATE TABLE unpaid_items (
    id BIGSERIAL PRIMARY KEY,
    person_name TEXT NOT NULL,
    amount NUMERIC(14,2) NOT NULL DEFAULT 0,
    description TEXT,
    settled BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE salaf_entries (
    id BIGSERIAL PRIMARY KEY,
    investor TEXT NOT NULL,
    direction TEXT NOT NULL CHECK (direction IN ('+', '-')),
    amount NUMERIC(14,2) NOT NULL CHECK (amount > 0),
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    created_by BIGINT REFERENCES users(id)
);

CREATE TABLE account_traffic (
    id BIGSERIAL PRIMARY KEY,
    account_id BIGINT NOT NULL REFERENCES accounts(id),
    person TEXT NOT NULL DEFAULT 'anonymous',
    amount NUMERIC(14,2) NOT NULL,
    occurred_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE audit_logs (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT REFERENCES users(id),
    user_name TEXT,
    area audit_area NOT NULL,
    action TEXT NOT NULL,
    title TEXT,
    details TEXT,
    metadata JSONB NOT NULL DEFAULT '{}',
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE register_tables (
    id BIGSERIAL PRIMARY KEY,
    name TEXT NOT NULL UNIQUE,
    description TEXT,
    created_by BIGINT REFERENCES users(id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE register_columns (
    id BIGSERIAL PRIMARY KEY,
    table_id BIGINT NOT NULL REFERENCES register_tables(id) ON DELETE CASCADE,
    key TEXT NOT NULL,
    header TEXT NOT NULL,
    position INTEGER NOT NULL DEFAULT 0,
    UNIQUE (table_id, key)
);

CREATE TABLE register_records (
    id BIGSERIAL PRIMARY KEY,
    table_id BIGINT NOT NULL REFERENCES register_tables(id) ON DELETE CASCADE,
    created_by BIGINT REFERENCES users(id),
    created_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE TABLE register_values (
    id BIGSERIAL PRIMARY KEY,
    record_id BIGINT NOT NULL REFERENCES register_records(id) ON DELETE CASCADE,
    column_id BIGINT NOT NULL REFERENCES register_columns(id) ON DELETE CASCADE,
    value TEXT,
    UNIQUE (record_id, column_id)
);

CREATE INDEX idx_account_transfers_occurred_at ON account_transfers(occurred_at);
CREATE INDEX idx_agency_links_agencies ON agency_links(agency_a_id, agency_b_id);
CREATE INDEX idx_agency_transfer_rules_agencies ON agency_transfer_rules(source_agency_id, destination_agency_id);
CREATE INDEX idx_inter_agency_transfers_status ON inter_agency_transfers(status);
CREATE INDEX idx_inter_agency_transfers_agencies ON inter_agency_transfers(source_agency_id, destination_agency_id);
CREATE INDEX idx_service_transactions_occurred_at ON service_transactions(occurred_at);
CREATE INDEX idx_audit_logs_created_at ON audit_logs(created_at);
