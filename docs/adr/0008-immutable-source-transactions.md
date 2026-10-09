# 0008. Source transactions are immutable; adjustments are separate

- Status: Accepted
- Date: 2026-10-09

## Context

Customers ask for reporting-date cutoffs, report exclusions and alternative tax views. Editing the original
sale to satisfy a report destroys reconciliation and auditability (proposal sections 2 and 18).

## Decision

- Sales, payments, returns, stock movements, GRNs and journals are insert-only. No procedure updates
  their business fields or deletes them.
- Corrections are new, linked records: return, void, credit note, stock adjustment.
- Reporting/tax transformations write to a separate layer that stores RuleVersionId, execution time,
  affected source ids, and source vs transformed totals. Reports expose both totals.
- Stock balances are derived from the `StockMovement` ledger and must reconcile to it.

## Consequences

- Tables for source transactions have no update procedures except status transitions that are themselves audited.
- Jurisdiction-specific tax treatment needs the customer's accountant to sign off before release.
