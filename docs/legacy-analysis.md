# Legacy VB.NET Analysis

## Source Layout

The original app is a .NET Framework 4.7.2 WinForms project in `RDET/RDET.vbproj`. It uses Guna UI, DevExpress reports/charts, LiveCharts, and `MySql.Data`.

Connection settings are created in `Class1.vb` from `My.Settings` and point at four MySQL databases:

- `security_db`: users, companies, accounts (`hisabat`)
- `damanecash`: services, service transactions, account transfers, cash counts, unpaid items, salaf, reports
- `trackerlines`: audit/history lines
- `resgister_db`: dynamic register tables/columns/values and client information

## Forms And Features

| Form/Class | Purpose |
| --- | --- |
| `LoginFrm` | Login, app connection settings panel, loads user profile image, writes login audit line. |
| `Form1` | Main shell/navigation and role gating (`Admin`, `Chef`, `User`). |
| `Home` | Daily/monthly service IN/OUT charts. |
| `DashBoard` | Account cards, balances, transfers, deposits/withdrawals, special account actions, transfer deletion. |
| `servicesManagement` | CRUD for services and fee bands (`fraisargumenttable`). |
| `tras` | Service transaction entry/update/delete for IN/OUT operations and fees. |
| `HisabatGestion` | Account/company/account-visibility management. |
| `CaiseCalcule` | Cash denomination count, real cash/non-paid reconciliation. |
| `CheckLesVersement` | Non-versement situation and bank/transfer reconciliation. |
| `Salaf`, `SalafDetailControl`, `FinancialSalafDetailContr` | Loan/investor detail tracking. |
| `usersFrm` | User, company, profile, password, image, and audit views. |
| `Register`, `register_inf`, `tableB` | Dynamic register/client information tables. |
| `rapports`, `JournalDC`, `XtraReport*`, `RP1` | DevExpress reports for journal, services, account extracts. |
| `TrackClass` | Audit stored procedure wrappers. |
| `AutoOperations` | Account historical balance snapshots. |
| `utilities` | Shared data access and core transaction helper logic. |
| `connectionSettings`, `Settings` | Local app settings and appearance. |
| `CleanTele`, `FileTransferModule` | Desktop file/folder utilities, not core web finance logic. |

## Old Database Objects Observed

Tables referenced from source:

- `users`, `companies`, `hisabat`
- `services`, `fraisargumenttable`
- `transactionsservices`, `tansactionscomptes`
- `compteshistoriquedata`
- `caissecalcul`, `nonpayedetail`
- `salafdetail`
- `detail_comptes_traffic`
- `relationsvompte`, `relationtypes`
- `userstrack` and tracker stored procedures
- `resgister_db.tables`, `columns`, `values`, `information_client`

Stored procedures referenced:

- `trans`, `UpTans`, `trans1`
- `GetTanslactionCompte`
- `GetBankDetail`
- `CheckIDServ`
- `insertTable`, `insertColumn`, `insertValue`
- `AddServiceLine2`, `AddTransLine2`, `AddUserLine2`
- `tranServicesReportByDate`, `GetFDIO`

## Business Rules Preserved

- Users have roles: `Admin`, `Chef`, `User`.
- Admin can manage services, accounts, users, companies, fees, and reports.
- Chef can access operational screens but not all admin editing.
- Account transfer subtracts from source account and adds to destination account.
- Service transactions choose source/destination accounts from service configuration.
- Some services have a `Para1` type. If transaction type matches `Para1`, use one account direction; otherwise reverse it.
- Fees are selected by amount range and service type.
- Deleting a service transaction reverses amount and fee movements.
- Cash count stores denomination counts per day and updates reconciliation accounts.
- `salaf` entries record investor/person movements and alter related accounts.
- Daily account snapshots are maintained for reporting.
- Audit lines are recorded for login/logout, CRUD, transactions, and prints.

## Problems Corrected In The Web Architecture

- Plaintext database credentials and app password in source config.
- Plaintext user passwords.
- SQL injection from string interpolation and raw user input.
- Shared global database connections.
- UI event handlers performing multi-table financial operations directly.
- Repeated transfer logic with hard-coded account IDs scattered across forms.
- Missing database transactions around balance updates.
- Silent `Catch ex As Exception` blocks that hide failures.
- Typo-heavy table names (`tansactionscomptes`, `resgister_db`) are normalized while mapping old concepts.

## Proposed PostgreSQL Schema

The new schema is in `database/init/001_schema.sql`. Core tables:

- `companies`
- `users`
- `accounts`
- `account_transfers`
- `services`
- `service_fee_rules`
- `service_transactions`
- `cash_counts`
- `unpaid_items`
- `salaf_entries`
- `account_traffic`
- `audit_logs`
- `register_tables`, `register_columns`, `register_records`, `register_values`

Financial writes are handled by backend service functions that run in a database transaction and update balances atomically.
