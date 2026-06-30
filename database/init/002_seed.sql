INSERT INTO companies (id, name, description)
VALUES (1, 'Default Company', 'Seed company for local development')
ON CONFLICT DO NOTHING;

INSERT INTO users (id, company_id, username, password_hash, first_name, last_name, role)
VALUES (
    1,
    1,
    'admin',
    crypt('admin123', gen_salt('bf')),
    'Admin',
    'User',
    'Admin'
)
ON CONFLICT DO NOTHING;

INSERT INTO accounts (legacy_id, name, balance, visible, company_ids) VALUES
    (1, 'Damane Pay', 0, TRUE, ARRAY[1]),
    (2, 'Fundex', 0, TRUE, ARRAY[1]),
    (3, 'Bank', 0, TRUE, ARRAY[1]),
    (4, 'Coffre', 0, TRUE, ARRAY[1]),
    (5, 'Caisse Calculee', 0, TRUE, ARRAY[1]),
    (7, 'Salaf', 0, TRUE, ARRAY[1]),
    (9, 'Capital', 0, TRUE, ARRAY[1]),
    (10, 'Factures Non Payees', 0, TRUE, ARRAY[1]),
    (11, 'Caisse Reelle', 0, TRUE, ARRAY[1]),
    (12, 'Non Verse Reelle', 0, TRUE, ARRAY[1]),
    (14, 'Salaf Autre', 0, TRUE, ARRAY[1]),
    (15, 'Virements En Attente', 0, TRUE, ARRAY[1]),
    (16, 'Caisse A Remise', 0, TRUE, ARRAY[1])
ON CONFLICT (legacy_id) DO NOTHING;
