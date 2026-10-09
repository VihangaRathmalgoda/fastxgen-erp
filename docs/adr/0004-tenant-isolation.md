# 0004. Tenant isolation: shared database, SESSION_CONTEXT and Row-Level Security

- Status: Accepted
- Date: 2026-10-09

## Context

Hundreds to thousands of tenants share the platform. A cross-tenant data leak is the most damaging failure
the product can have. With stored procedures there are no ORM query filters to rely on.

## Decision

1. Shared database, shared schema. Every tenant-owned table has `TenantId INT NOT NULL`.
2. The tenant is resolved only from the authenticated identity (`tenant_id` claim) into `ITenantContext`.
   Nothing in a request body, header or query string can set it.
3. When `SqlConnectionFactory` opens a connection it calls `sys.sp_set_session_context` for `TenantId`
   and `UserId` with `@read_only = 1`, so no later statement on that connection can change them.
4. Procedures read the tenant with `CAST(SESSION_CONTEXT(N'TenantId') AS INT)` and throw 50403 if it is null.
   They never accept `@TenantId` as a parameter (platform-admin procedures in the `platform` schema excepted,
   which are audited and reviewed separately).
5. Row-Level Security is the safety net: `security.TenantIsolationPolicy` adds a FILTER and a BLOCK predicate
   (`security.fn_TenantAccessPredicate`) to every tenant-owned table. A procedure that forgets its
   `WHERE TenantId = @TenantId` still cannot see or write another tenant's rows.
6. Unique keys and indexes lead with `TenantId`.
7. Enterprise tenants may get a dedicated database later; the same code works because the tenant still comes
   from SESSION_CONTEXT.

## Alternatives considered

- `@TenantId` parameter on every procedure: one missed check leaks data, and the value would come from C#
  callers that can be wrong. Rejected.
- Database per tenant for everyone: strong isolation, but heavy operations at thousands of tenants.

## Consequences

- Connection pooling resets session context between uses; an integration test must prove that a pooled
  connection never carries a previous tenant (Phase 1).
- Every module ships a tenant-isolation test (tenant A cannot read or write tenant B).
- Super Admin support access uses explicit, time-limited, audited sessions, not RLS bypass in normal endpoints.
