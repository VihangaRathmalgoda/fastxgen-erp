# Developer setup

## Prerequisites

- .NET SDK 10.0.401 or later 10.0.x (pinned in `global.json`)
- Git
- An editor: Visual Studio 2022 17.14+ / VS Code with C# Dev Kit / Rider
- Personal machine only (optional): SQL Server Developer/Express, LocalDB, or SQL Server in Docker

## First time

```powershell
git clone https://github.com/VihangaRathmalgoda/fastxgen-erp.git
cd fastxgen-erp
dotnet tool restore            # pinned tools (SqlPackage)
dotnet build FastXGen.Erp.slnx
```

## Everyday commands

```powershell
dotnet build FastXGen.Erp.slnx                                   # also validates every .sql file offline
dotnet test FastXGen.Erp.slnx                                    # database tests skip if no database is set
dotnet format FastXGen.Erp.slnx                                  # fix formatting before committing
```

## Web app (ADR 0015)

Needs Node.js 22 or later.

```powershell
cd src/FastXGen.Erp.Web
npm ci              # exact versions from package-lock.json
npm run dev         # http://localhost:5173 (/sign-in, /, /pos, /products, /rules)
npm run build       # type-check and production build into dist/
```

## Where a database may run (ADR 0011)

| Machine | Database |
|---|---|
| Office laptop | **None.** Build and run tests only; database tests skip themselves. |
| Personal machine | Your own local server, through the script below. |
| CI | A throwaway container, created and destroyed by GitHub Actions. |

### Local database on a personal machine

```powershell
./scripts/Publish-LocalDatabase.ps1 -ScriptOnly     # optional: read the SQL it will run
./scripts/Publish-LocalDatabase.ps1                 # LocalDB, database FastXGenErp_Dev
# SQL Server in Docker or a named instance:
./scripts/Publish-LocalDatabase.ps1 -Server "localhost,1433" -SqlLogin sa
```

The script refuses any server that is not on your machine and blocks changes that would lose data.

Then run every test, including database tests:

```powershell
$env:Database__ConnectionString = "Server=(localdb)\MSSQLLocalDB;Database=FastXGenErp_Dev;Integrated Security=True;TrustServerCertificate=True"
dotnet test FastXGen.Erp.slnx
```

And give the API the same connection string through user secrets (never a file in the repo):

```powershell
dotnet user-secrets init --project src/FastXGen.Erp.Api
dotnet user-secrets set "Database:ConnectionString" "<connection string>" --project src/FastXGen.Erp.Api
dotnet run --project src/FastXGen.Erp.Api
```

## SQL files for someone else to run

```powershell
./scripts/Build-DatabaseScripts.ps1
```

Writes to `artifacts/sql/` (git-ignored), without connecting to any server:

- `FastXGenErp_Create_<timestamp>.sql`: creates a new database with every object, in dependency order.
  It **drops** an existing database of the same name; for empty, approved environments only.
  Run in SQLCMD mode (`sqlcmd -i <file>`, or SSMS with Query > SQLCMD Mode).
- `FastXGen.Erp.Database.dacpac`: used to generate an upgrade script for an existing database.

## Running the API without a database

`/api/v1/system/ping` and `/health/live` work; `/health/ready` reports the database as unhealthy.

## Continuous integration

GitHub Actions (`.github/workflows/ci.yml`) runs on every pull request and push to `main`: format check,
build, all tests, deployment to a throwaway SQL Server, database tests, and a vulnerable-package scan.
The generated SQL scripts are attached to each run as the `database-scripts` artifact.
