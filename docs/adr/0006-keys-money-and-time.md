# 0006. Keys, money and time types

- Status: Accepted
- Date: 2026-10-09

## Decision

| Concern | Rule |
|---|---|
| Tenant key | `TenantId INT` |
| Entity keys | `<Entity>Id BIGINT IDENTITY`, primary key clustered on `(TenantId, <Entity>Id)` for tenant tables |
| Client-generated ids (offline POS, retries) | `ClientRequestId UNIQUEIDENTIFIER` with a unique index per tenant (idempotency) |
| Money, quantity | `DECIMAL(18,4)`; C# `decimal`. Never `float`, `real` or `money` |
| Rates, percentages | `DECIMAL(9,6)` |
| Rounding | Explicit `MidpointRounding` per tenant/currency setting; never implicit |
| Instants | `DATETIME2(3)` in UTC, named `...AtUtc`; C# `DateTimeOffset` |
| Business dates | `DATE` (`InvoiceDate`, `ReportingDate`); C# `DateOnly` |
| Concurrency | `RowVersion ROWVERSION` on rows edited by more than one user |
| Soft delete | Master data only (`IsActive`, `DeactivatedAtUtc`). Transactions are never deleted (ADR 0008) |
| Text | `NVARCHAR` everywhere (multi-language), with explicit lengths |

## Alternatives considered

- GUID primary keys: no central sequence, but random GUIDs fragment clustered indexes and SQL Server does not
  sort UUIDv7 in creation order. Kept for idempotency only.

## Consequences

- API ids are numbers; enumeration is harmless because RLS and scope checks hide other tenants' rows.
