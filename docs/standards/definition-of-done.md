# Definition of done

A slice is done only when every box is ticked.

## Every slice

- [ ] Plan was approved before coding.
- [ ] Code follows `CLAUDE.md`; the build passes with no warnings; `dotnet format --verify-no-changes` passes.
- [ ] Unit tests for the use case, including failure paths, with explicit expected values.
- [ ] Every new endpoint has a permission policy, with allowed and denied tests.
- [ ] Every new tenant-owned table has `TenantId`, is in `TenantIsolationPolicy`, and has a cross-tenant test.
- [ ] Every state change writes an audit event in the same transaction.
- [ ] Lists are paged; indexes support the filters and sort used.
- [ ] No source transaction is updated or deleted.
- [ ] Database changes reviewed for locking and data impact; no destructive change in one step.
- [ ] ADR added or updated if an architectural decision was made.
- [ ] `docs/progress.md` updated.

## Extra for money, stock and tax

- [ ] Rounding is explicit and tested at the boundary (0.005 cases).
- [ ] Stock balances reconcile to movements in a test.
- [ ] Concurrency (stale RowVersion) and idempotency (repeated ClientRequestId) tested.

## Commercial release (proposal section 32)

- [ ] Every module has authorization and tenant-isolation tests.
- [ ] POS sales and returns reconcile; inventory ledger reconciles to balances.
- [ ] Reports trace back to source transactions.
- [ ] Configuration changes are versioned and reversible.
- [ ] Backups restored in a test, monitoring and alerting live, API and user docs published.
- [ ] Load, security and UAT testing done for the launch scale.
