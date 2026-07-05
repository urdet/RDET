# RDET Web

Modern web rebuild of the legacy VB.NET WinForms RDET application.

The old application managed Damane Cash/service transactions, account balances, cash counting, unpaid amounts, loans (`salaf`), user/company administration, audit trails, reports, and a flexible register. This rebuild keeps those business concepts but moves the app to a clean web architecture:

- React + Vite + Tailwind frontend
- FastAPI backend
- PostgreSQL database
- Docker Compose for local development

## Project Structure

```text
backend/      FastAPI API, SQLAlchemy models, business services
frontend/     React/Vite/Tailwind web client
database/     PostgreSQL schema and seed data
docs/         Legacy VB.NET analysis and migration notes
RDET/         Original VB.NET source retained for reference
```

## Legacy Findings

See [docs/legacy-analysis.md](docs/legacy-analysis.md) for the form/module inventory, old database usage, business rules, and security problems found in the VB.NET source.

## Local Development

1. Copy environment defaults if you want to customize them:

   ```bash
   cp backend/.env.example backend/.env
   ```

2. Start the stack:

   ```bash
   docker compose up --build
   ```

3. Open the apps:

   - Frontend: http://localhost:5173
   - Backend API: http://localhost:8000
   - API docs: http://localhost:8000/docs
   - PostgreSQL: `localhost:5433`, database `rdet`, user `rdet`, password `rdet_dev_password`

## Access From Another PC On The Same Network

1. Start the stack on the host PC:

   ```bash
   docker compose up --build
   ```

2. Find the host PC IPv4 address, then open the frontend from another PC:

   ```text
   http://HOST_PC_IP:5173
   ```

   Example:

   ```text
   http://192.168.1.25:5173
   ```

The frontend automatically calls the backend at `http://HOST_PC_IP:8000`. If another PC cannot connect, allow inbound traffic to ports `5173` and `8000` in the host PC firewall.

To force a specific backend URL, set `VITE_API_URL` before starting Docker Compose.

## Default Login

The seed data creates:

- Username: `admin`
- Password: `admin123`

Change this before using real data.

## Migration Notes

The VB.NET code used several MySQL databases (`security_db`, `damanecash`, `trackerlines`, `resgister_db`) and stored credentials in `App.config`. The web version consolidates those concepts into one PostgreSQL schema with explicit foreign keys, hashed passwords, audit trails, and API-level validation.

The first implemented modules are authentication/session bootstrap, accounts, services/fees, account transfers, service transactions, cash count snapshots, unpaid items, salaf entries, register definitions/records, dashboard summaries, and audit logs. Reports are exposed as API summaries and can later be extended to PDF export.
