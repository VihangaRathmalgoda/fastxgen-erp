## What and why

<!-- One or two sentences. Link the issue. -->

## Slice

- Module:
- Use cases / endpoints:
- Database objects added or changed:

## Checklist (docs/standards/definition-of-done.md)

- [ ] Plan approved before coding
- [ ] Unit tests incl. failure paths, explicit expected values
- [ ] Permission policy + allowed/denied tests on every new endpoint
- [ ] TenantId + RLS policy + cross-tenant test on every new tenant table
- [ ] Audit event in the same transaction as each change
- [ ] Lists paged and indexed
- [ ] No source transaction updated or deleted
- [ ] No business decision in SQL; no inline SQL in C#
- [ ] ADR added/updated if a decision was made
- [ ] `docs/progress.md` updated

## Database impact

<!-- Large tables touched? Locks? Data migration? Reversible? -->
