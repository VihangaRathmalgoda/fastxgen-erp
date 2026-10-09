# 0005. C# decides, SQL stores

- Status: Accepted
- Date: 2026-10-09

## Context

Stored-procedure systems become hard to maintain when the same rule exists in C# and in SQL, or when
business decisions hide in procedures where they cannot be unit tested.

## Decision

| Belongs in C# (Domain / Application) | Belongs in SQL (procedures) |
|---|---|
| Is this discount allowed for this role and amount? | Constraints: keys, foreign keys, NOT NULL, CHECK |
| Which price list, tax rule or business rule applies? | Atomic multi-table writes in one transaction |
| Totals, tax and rounding calculation | Stock posting to the movement ledger and balance update |
| Workflow and approval state transitions | Concurrency checks (RowVersion) and idempotency checks |
| Input validation and error messages | Set-based reads, search, paging, reports, reconciliation |

- A procedure may re-check an invariant that protects data (for example "stock posting must reference an
  existing product of this tenant"), but it must not decide business policy.
- Calculated values (line totals, tax) are computed in C#, sent to the procedure, and stored as sent.

## Consequences

- Business rules are unit-testable without a database.
- Procedures stay short and predictable; a DBA can tune them without changing behaviour.
