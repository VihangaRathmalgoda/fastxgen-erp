# FastXGen ERP: Engineering Rules

Read this file and `docs/adr/` at the start of every session. These rules are non-negotiable.
To change one, write a new ADR first (`docs/adr/`), then change the code.

## 1. Architecture (ADR 0001)

Modular monolith, Clean Architecture. Projects and what they may reference:

| Project | Holds | May reference |
|---|---|---|
| `FastXGen.Erp.Domain` | Entities, value objects, domain rules | nothing |
| `FastXGen.Erp.Contracts` | API request/response records | nothing |
| `FastXGen.Erp.Application` | Use cases, ports (interfaces), validation | Domain, Contracts |
| `FastXGen.Erp.Infrastructure` | Clock, identity, email, storage, integrations | Application |
| `FastXGen.Erp.Persistence` | Repositories that call stored procedures | Application |
| `FastXGen.Erp.Api` | Controllers, middleware, composition root | all of the above |
| `database/FastXGen.Erp.Database` | Tables, procedures, views, types (SQL project) | n/a |

- Code is organised by module inside each layer: `Features/<Module>/<UseCase>` in Application,
  `Repositories/<Module>` in Persistence, `Controllers/V1/<Module>Controller` in Api, `<schema>/` in the database.
- Controllers are thin: bind request, call one Application use case, return the result. No logic, no SQL.
- The architecture tests (`tests/FastXGen.Erp.ArchitectureTests`) enforce this. Never weaken a test to pass a build.

## 2. Database and stored procedures (ADR 0002, 0003, 0005)

- All data access in the whole product goes through stored procedures (modules, reports, auth look-ups, jobs).
  No inline SQL, no EF Core, no dynamic SQL in C# or in procedures.
- Only `Persistence/Sql/SqlExecutor` talks to SQL Server. Repositories call it with a `StoredProcedureName`.
- Naming: `<schema>.usp_<Entity>_<Action>`, for example `sales.usp_Invoice_Create`. One object per `.sql` file.
- **C# decides, SQL stores.** Business decisions (may this discount apply? which rule wins?) live in Domain/Application.
  Procedures validate integrity, persist atomically, post stock, and run set-based reads and reports.
  Never implement the same rule in both places.
- Each write procedure owns its transaction (`SET XACT_ABORT ON; BEGIN TRAN ... COMMIT`) and writes its audit row
  inside that same transaction.
- Expected failures use `THROW` with: 50403 forbidden, 50404 not found, 50409 conflict, 50422 business rule.
- Collections (invoice lines etc.) are passed as table-valued parameters, never as CSV or JSON strings.
- Use the procedure template in `docs/standards/database-standards.md`.
- **Where SQL may run (ADR 0011):** office laptop: never. Personal machine: only your own local server, only via
  `scripts/Publish-LocalDatabase.ps1`. Shared/production: reviewed scripts run by the authorised person.

## 3. Multi-tenancy (ADR 0004): highest risk area

- Every tenant-owned table has `TenantId INT NOT NULL` and is covered by `security.TenantIsolationPolicy`
  (Row-Level Security filter + block predicates).
- Tenant and user come from the authenticated identity only (`ITenantContext`). Never from body, header, query.
- The API stamps `TenantId`/`UserId` into `SESSION_CONTEXT` (read-only) when it opens a connection.
  Procedures read the tenant from `SESSION_CONTEXT`, never from a parameter.
- Unique constraints and indexes start with `TenantId` (then `BranchId` and date/status where relevant).
- Background jobs, cache keys and file paths always carry the tenant.

## 4. Data integrity (ADR 0006, 0008)

- Source transactions (sales, payments, stock movements, GRNs) are never updated or deleted.
  Corrections are new records (return, void, adjustment) linked to the original.
- Reporting/tax changes live in a separate transformation layer; reports show source and transformed totals.
- Stock is an append-only `StockMovement` ledger; balances are derived and must reconcile.
- Ids (ADR 0006): users, roles, sessions, terminals, API keys = `UNIQUEIDENTIFIER DEFAULT NEWSEQUENTIALID()`;
  business records = `BIGINT IDENTITY` (never shown); documents get a display number (`INV-BR03-2026-000123`);
  tenant = `TenantId INT` internally + `TenantKey UNIQUEIDENTIFIER` publicly.
- Types: money and quantity `DECIMAL(18,4)`, rates `DECIMAL(9,6)`; never `float`/`real`/`money`.
  Instants `DATETIME2(3)` in UTC named `...AtUtc`; business dates `DATE`.
- Concurrency-sensitive rows have `RowVersion ROWVERSION`; updates pass the version and fail with 50409 if stale.
- POS and payment endpoints require an idempotency key.

## 5. Security

- Permissions are `Module.Resource.Action` (for example `Sales.Invoice.Create`).
- Every endpoint has `[Authorize(Policy = "...")]` or an explicit `[AllowAnonymous]` (enforced by test).
- Branch / own-records scope is enforced in the procedure query, not only in the UI.
- No secrets in source or appsettings. Connection strings come from environment or a secret store.
- Log no passwords, tokens, card data or full personal data. Audit logins, permission changes, exports, config changes.

## 5a. Input validation (ADR 0012)

- Treat every input as hostile. Every `*Request` contract has a FluentValidation validator in Application
  (build fails otherwise); `ValidationFilter` runs it automatically and fails closed.
- Use the shared rules in `ValidationRules` (`PlainText`, `Code`, `Money`, `Quantity`, `Percentage`, `EntityId`,
  `Email`, `AppLink`); no ad-hoc regexes in validators. Route ids use constraints: `{id:long:min(1)}`.
- Input created inside the app (jobs, imports, notifications) is validated with `ValidateOrThrowAsync`.
- Never relax the JSON settings (unknown properties rejected), size limits, rate limits or security headers.

## 5b. Messages, alerts and notifications (ADR 0013)

- Errors are ProblemDetails with a stable `code`, a user-safe `detail` and `correlationId`. Never return
  exception text. Business failures: `throw new BusinessRuleException("module.reason", "User-facing text")`.
- Success with a caveat: add `messages: UserMessage[]` to the response contract.
- Telling a user about something later: `INotificationPublisher.PublishAsync(NotificationDraft)`.

## 6. Rules engine (ADR 0009)

- Rules are versioned JSON metadata (conditions + actions), evaluated in C#.
- Lifecycle: draft, validate, simulate, publish; rollback by publishing a new version.
- Every rule application is traced (RuleVersionId, time, affected ids). Never executable code or SQL from users.

## 7. API (ADR 0010)

- Routes under `/api/v1/...`. Errors are ProblemDetails with `correlationId`.
- Every list endpoint is paginated (`page`, `pageSize` max 200) and filtered server-side.

## 8. Code style

- Follow `.editorconfig` and `docs/standards/coding-standards.md`. Build has warnings as errors.
- Small classes, one public type per file, file-scoped namespaces, `async` all the way with `CancellationToken`.
- Names say what things are. No `Helper`, `Manager`, `Utils`, `Common2`. No commented-out code. No TODO without an issue.
- Package versions live only in `Directory.Packages.props`. **Ask before adding any NuGet or npm package.**

## 9. Tests (definition of done: `docs/standards/definition-of-done.md`)

- Every use case has unit tests, including failure paths.
- Every module has: a tenant-isolation test (tenant A cannot read or write tenant B),
  and an authorization test (allowed and denied).
- Money, tax, rounding, stock and rule logic is tested first, with explicit expected numbers.
- Database tests carry `[Trait("Category", "Database")]` and skip when no database is configured
  (office laptop); they run in CI and on a personal machine with a local database.

## 10. Workflow

- One small vertical slice per branch and PR. Branch names: `feature/<module>-<slice>`, `fix/...`, `chore/...`.
- Before coding: write a short plan and wait for approval.
- After coding: run the review prompts in `docs/standards/review-checklist.md`; update `docs/progress.md`.
- Any architectural decision: add or update an ADR in the same PR.
- Commit messages: Conventional Commits (`feat(sales): ...`, `fix(inventory): ...`, `chore: ...`).
