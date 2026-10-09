# 0012. Input validation and API hardening

- Status: Accepted
- Date: 2026-10-09

## Context

An ERP holds money, stock and personal data for many tenants, and is exposed to the internet.
Every value a client sends is untrusted: it may be malformed, oversized, or crafted to inject script,
SQL, or extra fields. Validation that each developer remembers to add will be forgotten somewhere.

## Decision

**Validation is mandatory and automatic (FluentValidation).**

1. Every request contract (`*Request` in `FastXGen.Erp.Contracts`) has a validator in Application.
   An architecture test fails the build if one is missing.
2. `ValidationFilter` runs the validator for every complex action argument before the action executes.
   It **fails closed**: an argument without a validator is a 500, never silently accepted.
3. Route ids use constraints (`{id:long:min(1)}`); simple query values are bound into a request record.
4. Field types use shared rules from `ValidationRules`, never ad-hoc checks:

   | Rule | Use for | Blocks |
   |---|---|---|
   | `PlainText(max)` | names, descriptions, notes | control chars, `<` `>`, bidi-override spoofing, over-length |
   | `Code(max)` | SKU, branch code, prefixes | anything but letters, digits, `.` `-` `_` |
   | `Money()` / `Quantity()` / `Percentage()` | amounts | negatives, overflow, precision beyond DECIMAL(18,4)/(9,6) |
   | `EntityId()` | foreign ids | zero and negative ids |
   | `Email()` | e-mail addresses | invalid or over 254 chars |
   | `AppLink()` | links stored for later display | absolute URLs, `//host`, `javascript:` (open redirect, XSS) |

5. Input produced inside the application (jobs, imports, notifications) is validated with
   `validator.ValidateOrThrowAsync(...)`, raising `RequestValidationException`.
6. The database is the last line of defence: parameterised stored procedures only (no SQL injection path),
   column lengths, CHECK constraints, and RLS.

**The HTTP surface is hardened.**

| Control | Setting |
|---|---|
| Unknown JSON properties | Rejected (`UnmappedMemberHandling.Disallow`), stops mass-assignment tricks |
| JSON parser messages | Hidden from clients |
| JSON depth / enums | Max depth 32; enums only as names, not numbers |
| Body / header size | 1 MB body, 32 KB headers (Kestrel limits in appsettings) |
| Rate limiting | 300 requests/min per user or IP (configurable); health checks exempt; 429 ProblemDetails |
| CORS | Only origins listed in `Cors:AllowedOrigins`; none by default |
| Response headers | `nosniff`, `X-Frame-Options: DENY`, CSP `default-src 'none'`, `no-referrer`, `Cache-Control: no-store` on API |
| Server banner | Removed |
| Transport | HTTPS redirection; HSTS outside Development |

## Consequences

- Adding an endpoint means adding a validator; the build enforces it.
- Output encoding remains the client's job (React escapes by default); `PlainText` reduces stored-XSS risk
  but does not replace encoding.
- Authentication-specific protections (lockout, MFA, token rotation) come with ADR 0014.
- Request size limits are enforced by Kestrel; in-memory test servers do not apply them.
