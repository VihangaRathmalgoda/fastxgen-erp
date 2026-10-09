# Progress

## Done

- **Phase 0, foundation** (`chore/foundation`)
  - Rules: `CLAUDE.md`, ADRs 0001 to 0013, standards (coding, database, git, definition of done, review).
  - Solution: Domain, Contracts, Application, Infrastructure, Persistence, Api; Unit, Architecture and
    Integration tests; central package versions; analyzers with warnings as errors.
  - Data access: stored procedures only through `SqlExecutor` (validated names), `SqlConnectionFactory`
    (tenant/user into read-only SESSION_CONTEXT), `SqlErrorTranslator` (THROW codes to exceptions).
  - Validation (ADR 0012): FluentValidation, shared `ValidationRules`, fail-closed `ValidationFilter`,
    build-time check that every request has a validator.
  - Hardening: unknown JSON fields rejected, parser messages hidden, size limits, rate limiting,
    CORS allow-list, security headers, no server banner, HSTS.
  - Messages (ADR 0013): one ProblemDetails shape with `code` and `correlationId` for every error;
    `UserMessage` and `MessageSeverity` for inline messages.
  - Notifications: `notify.Notification` + 4 procedures, first table under `security.TenantIsolationPolicy`,
    `INotificationPublisher` (validated) and `NotificationStore`.
  - Database tooling: offline build, create script, `Publish-LocalDatabase.ps1` for personal machines.
  - CI: format, build, tests, throwaway SQL Server, database tests, vulnerability scan.
  - Tests: 90 passing without a database; 8 database tests (tenant isolation, session context) run in CI.

## Decisions pending

- ADR 0014, authentication: ASP.NET Core Identity inside the API (users stored through our procedures)
  vs an external identity provider (Entra ID / Auth0 / Keycloak). Needed before Phase 1.
- ADR 0015, frontend: UI kit (MUI / Ant Design / custom) and whether offline POS is in the MVP.

## Known shortcuts

- Notification endpoints, real-time push and e-mail/SMS delivery wait for authentication (ADR 0013).
- `ERP.Reporting` and `ERP.BackgroundJobs` projects are deferred until first needed (ADR 0001).

## Next slice

Phase 1.1, tenant and organisation core: `core.Tenant`, `core.Branch`, `core.Warehouse`, `core.Terminal`
(each added to `TenantIsolationPolicy`), `audit.AuditEvent` + `audit.usp_AuditEvent_Write`,
with validators, procedures and cross-tenant tests. Work on the personal machine with a local database.
