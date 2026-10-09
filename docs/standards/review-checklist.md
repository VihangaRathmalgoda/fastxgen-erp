# Review checklist

Run these after every slice, before opening or approving a pull request.
Each one can be pasted as a prompt to an AI reviewer or used by a human reviewer.

## R1. Slice review

```
Review the changes on this branch against CLAUDE.md and docs/adr. Check:
1. Every new tenant-owned table has TenantId, is in security.TenantIsolationPolicy, and has a cross-tenant test.
2. Every procedure reads the tenant from SESSION_CONTEXT, never from a parameter, and filters by TenantId.
3. Every endpoint has a permission policy and allowed/denied tests.
4. Audit events are written inside the same transaction as the change.
5. Money/quantity use DECIMAL(18,4) / decimal; rounding is explicit and tested.
6. List procedures are paged; indexes lead with TenantId and match the filters.
7. No source transaction is updated or deleted.
8. No business decision is implemented in SQL; no rule exists in both C# and SQL.
9. No inline SQL in C#; repositories use ISqlExecutor with schema.usp_Entity_Action names.
10. Database changes: locking on large tables, defaults, and whether the change is reversible.
List problems by severity with file and line. Do not fix anything yet.
```

## R2. Test quality

```
Review the tests added on this branch. Find tests that would still pass if the code were wrong
(asserting not-null, mocks that hide behaviour, missing negative cases). Propose the missing cases for
tax, pricing, rounding, stock, permission and tenant-isolation logic.
```

## R3. Drift check

```
Compare the code structure with CLAUDE.md and docs/adr. List every deviation: wrong layer, new dependency,
new pattern, undocumented decision, naming that breaks the standards. For each, propose a code fix or an ADR.
```

## R4. Session handoff

```
Update docs/progress.md: what is done, decisions made, open questions, known shortcuts, and the exact next
slice to build. Keep it under 60 lines.
```
