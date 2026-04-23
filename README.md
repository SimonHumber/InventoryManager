# Inventory Manager

ASP.NET Core MVC + Web API microservice. Run locally with the steps below.

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

## Install & run

```bash
git clone <repo-url>
cd Final
dotnet restore InventoryManagement.slnx
./scripts/run-dev.sh
```

This starts both services and creates/seeds the SQLite databases automatically:

- Web (MVC): <http://localhost:5100>
- API (microservice): <http://localhost:5163>

## Default accounts (seeded)

| Role  | Email                   | Password     |
| ----- | ----------------------- | ------------ |
| Admin | admin@inventory.local   | Admin#12345  |
| User  | user@inventory.local    | User#12345   |

## Run each project manually (optional)

```bash
# Terminal 1 — microservice
dotnet run --project src/Inventory.Api --urls http://localhost:5163

# Terminal 2 — MVC web app
dotnet run --project src/Inventory.Web --urls http://localhost:5100
```

## Run with Docker (optional)

```bash
docker compose up --build
```

Same URLs as above.
