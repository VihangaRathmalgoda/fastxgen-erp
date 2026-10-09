# FastXGen ERP

Multi-tenant SaaS ERP for retail shops, distributors, service businesses and multi-branch organisations:
POS, sales, purchasing, inventory, finance, tax and configurable reporting.

**Stack:** ASP.NET Core (.NET 10) Web API · SQL Server with stored procedures (Dapper) · React + TypeScript (planned)

## Start here

| Read | For |
|---|---|
| [CLAUDE.md](CLAUDE.md) | The non-negotiable engineering rules |
| [docs/adr](docs/adr/README.md) | Why the architecture is the way it is |
| [docs/standards](docs/standards) | Coding, database, git, definition of done, review checklist |
| [docs/dev-setup.md](docs/dev-setup.md) | Build, test, generate SQL scripts |
| [docs/progress.md](docs/progress.md) | What is done and what comes next |
| [docs/product](docs/product) | Product proposal and client feature guide |

## Repository layout

```
src/
  FastXGen.Erp.Domain           entities, value objects, domain rules (no dependencies)
  FastXGen.Erp.Contracts        API request/response types (no dependencies)
  FastXGen.Erp.Application      use cases and ports
  FastXGen.Erp.Infrastructure   clock, identity, email, storage, integrations
  FastXGen.Erp.Persistence      repositories calling stored procedures
  FastXGen.Erp.Api              controllers, middleware, composition root
database/
  FastXGen.Erp.Database         tables, procedures, types (SQL project, builds offline)
tests/
  FastXGen.Erp.UnitTests
  FastXGen.Erp.ArchitectureTests
  FastXGen.Erp.IntegrationTests
```

## Quick start

```powershell
dotnet tool restore
dotnet build FastXGen.Erp.slnx
dotnet test FastXGen.Erp.slnx            # database tests skip when no database is configured
./scripts/Build-DatabaseScripts.ps1      # SQL files in artifacts/sql, no server needed
```
