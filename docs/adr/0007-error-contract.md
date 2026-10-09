# 0007. Error contract: THROW codes to ProblemDetails

- Status: Accepted
- Date: 2026-10-09

## Decision

Expected failures travel as typed errors from SQL to HTTP:

| Procedure raises | C# exception | HTTP |
|---|---|---|
| `THROW 50403` | `ForbiddenException` | 403 |
| `THROW 50404` | `NotFoundException` | 404 |
| `THROW 50409`, or unique key violation 2601/2627 | `ConflictException` | 409 |
| `THROW 50422` | `BusinessRuleException` | 422 |
| anything else | unchanged | 500, no internal details |

- `SqlErrorTranslator` does the mapping; `AppExceptionHandler` writes ProblemDetails.
- Every error response includes `correlationId`, also returned in the `X-Correlation-Id` header and logged.
- Messages in `THROW 50422` are user-facing; never include table, constraint or SQL text.

## Consequences

- Clients get one error shape for every endpoint.
- Duplicate-key messages are replaced with a neutral text so constraint names do not leak.
