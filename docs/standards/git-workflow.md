# Git workflow

## Branches

- `main` is always releasable and protected: changes arrive only by pull request with green CI.
- One branch per vertical slice, short-lived (days, not weeks):
  - `feature/<module>-<slice>` e.g. `feature/inventory-stock-movement`
  - `fix/<module>-<issue>` e.g. `fix/sales-rounding-on-returns`
  - `chore/<topic>` e.g. `chore/foundation`, `docs/<topic>`

## Commits

Conventional Commits, imperative mood, one logical change per commit:

```
feat(sales): post POS sale with idempotency key
fix(inventory): reject transfer to the same warehouse
chore(db): add catalog schema
docs(adr): record authentication decision
test(sales): cover split payment rounding
```

Never commit generated output (`bin/`, `obj/`, `.dacpac`), secrets or local settings.

## Pull requests

- Small: aim for under 400 changed lines excluding generated SQL.
- Fill in the PR template; link the ADR if a decision changed.
- Self-review against `review-checklist.md` before asking for review.
- At least one approval. The author merges with **squash**, using a Conventional Commit title.
- CI must be green: format, build (warnings as errors), unit, architecture, integration and database tests.
