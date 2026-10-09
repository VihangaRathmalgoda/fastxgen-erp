# 0011. Testing strategy and where databases may run

- Status: Accepted
- Date: 2026-10-09

## Context

Stored procedures and tenant isolation must be tested against a real SQL Server. Some development happens on
office laptops where creating databases is not permitted; other development happens on personal machines
where a local SQL Server is fine.

## Decision

| Project | Tests | Needs a database |
|---|---|---|
| `UnitTests` | Validation rules, domain rules, calculations, use cases (ports faked) | No |
| `ArchitectureTests` | Layer rules, endpoint access rules, route versioning, validator coverage | No |
| `IntegrationTests` | HTTP pipeline in memory (validation, errors, security headers, rate limits) | No |
| `IntegrationTests` with `[Trait("Category", "Database")]` | Procedures, tenant isolation, session context | Yes |

Where a database may exist:

| Machine | Database allowed | How |
|---|---|---|
| Office laptop | **No** | `dotnet test --filter "Category!=Database"`; database tests skip themselves |
| Personal machine | Yes, own local server only | `scripts/Publish-LocalDatabase.ps1` (refuses non-local servers), then set `Database__ConnectionString` |
| CI | Yes, throwaway container | GitHub Actions deploys the dacpac and runs every test |
| Shared / production | Never from a developer machine | Reviewed scripts run by the authorised person (ADR 0003) |

- Database tests skip (not fail) when `Database__ConnectionString` is not set.
- Each test uses fresh random tenant ids, so tests never depend on each other's data.
- Each module adds: tenant-isolation tests, authorization allowed/denied tests, and procedure tests for
  concurrency (stale RowVersion) and idempotency (repeated ClientRequestId).
- Money, tax and stock tests assert exact numbers; a test that only checks "not null" is not a test.

## Consequences

- On the office laptop, SQL changes get feedback from CI: push early.
