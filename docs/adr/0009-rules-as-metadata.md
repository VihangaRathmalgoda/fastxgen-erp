# 0009. Business rules as versioned JSON metadata

- Status: Accepted
- Date: 2026-10-09

## Decision

- A rule is JSON: conditions (date/time, branch, terminal, role, product/category, customer group, amount,
  payment type) and actions (reporting date shift, exclusion, approval requirement, discount limit, ...).
- Rules are stored as rows (`rules.BusinessRule`, `rules.RuleVersion`) and evaluated in C# (ADR 0005).
- Lifecycle: draft, validate, simulate, publish. Rollback = publish a new version or deactivate.
- Every application of a rule is logged with RuleVersionId, time and affected ids, so the system can answer
  "why did this happen?".
- Users can never upload executable code, expressions that compile to code, or SQL.

## Consequences

- Start with two rule types (cutoff reporting date, product/category exclusion); add types only with tests.
