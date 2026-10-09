# Architecture Decision Records

An ADR records one significant decision: the context, what we chose, what we rejected and the consequences.
ADRs are never edited after acceptance; a later ADR supersedes an earlier one.

New ADR: copy `0000-template.md`, take the next number, open it in the same PR as the code it affects.

| # | Decision | Status |
|---|---|---|
| [0001](0001-modular-monolith-clean-architecture.md) | Modular monolith with Clean Architecture | Accepted |
| [0002](0002-stored-procedures-with-dapper.md) | All data access through stored procedures, called with Dapper | Accepted |
| [0003](0003-database-as-code.md) | Database as code: SQL project, offline build, reviewed scripts | Accepted |
| [0004](0004-tenant-isolation.md) | Tenant isolation: shared database, SESSION_CONTEXT and Row-Level Security | Accepted |
| [0005](0005-business-logic-placement.md) | C# decides, SQL stores | Accepted |
| [0006](0006-keys-money-and-time.md) | Identifiers (identity GUIDs, record ids, display numbers), money and time | Accepted |
| [0007](0007-error-contract.md) | Error contract: THROW codes to ProblemDetails | Accepted |
| [0008](0008-immutable-source-transactions.md) | Source transactions are immutable; adjustments are separate | Accepted |
| [0009](0009-rules-as-metadata.md) | Business rules as versioned JSON metadata | Accepted |
| [0010](0010-api-conventions.md) | API conventions | Accepted |
| [0011](0011-testing-strategy.md) | Testing strategy and where databases may run | Accepted |
| [0012](0012-input-validation-and-api-hardening.md) | Input validation and API hardening | Accepted |
| [0013](0013-user-messages-and-notifications.md) | User messages, alerts and notifications | Accepted |
| [0014](0014-authentication.md) | Authentication: ASP.NET Core Identity with stored-procedure stores | Accepted |
| [0015](0015-frontend-stack.md) | Frontend: React + TypeScript, Vite, custom CSS (offline POS open) | Accepted |
