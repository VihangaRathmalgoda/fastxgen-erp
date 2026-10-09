# 0010. API conventions

- Status: Accepted
- Date: 2026-10-09

## Decision

- Controllers (not minimal APIs), one per module resource, under `Controllers/V1`, routed at `api/v1/...`.
- Every action carries `[Authorize(Policy = "<Module>.<Resource>.<Action>")]` or `[AllowAnonymous]`.
  An architecture test fails the build otherwise.
- Request/response types live in `FastXGen.Erp.Contracts`; domain types never leave the API.
- Lists: `GET ...?page=1&pageSize=50` (max 200), returning `{ items, page, pageSize, totalCount }`.
- Concurrency: updates send the `rowVersion` they read; stale versions return 409.
- Idempotency: POS sales and payments require an `Idempotency-Key` header, stored as `ClientRequestId`.
- Errors: ProblemDetails (ADR 0007). Health: `/health/live` (process) and `/health/ready` (database).
- OpenAPI document generated at `/openapi/v1.json` in Development.
