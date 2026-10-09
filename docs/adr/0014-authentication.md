# 0014. Authentication: ASP.NET Core Identity with stored-procedure stores

- Status: Accepted
- Date: 2026-10-09

## Context

Users are owners, managers, back-office staff and cashiers of many small and mid-sized businesses.
Cashiers switch quickly on shared tills and need a PIN login. All data access must go through stored
procedures (ADR 0002). Hosted identity providers charge per monthly active user, which is costly for many
small tenants, and handle till-style PIN login poorly. Enterprise customers will want single sign-on later.

## Decision

1. **ASP.NET Core Identity core** (`UserManager`, `SignInManager`, password hasher, lockout, TOTP two-factor)
   inside the API, with **custom stores** that call stored procedures in the `identity` schema.
   No EF Core stores. No custom cryptography.
2. **Keys:** users, roles, sessions and terminals use identity keys (`UNIQUEIDENTIFIER`, ADR 0006).
   Users belong to one tenant; e-mail/username is unique per tenant.
3. **Tokens:**
   - Access token: JWT, 15 minutes, claims `sub` (user key), `tenant_id`, `sid` (session key),
     and permissions resolved at sign-in.
   - Refresh token: 256-bit random value, stored as a SHA-256 hash in `identity.UserSession`, rotated on every
     use. Reuse of an already-rotated token revokes the whole session family (theft detection).
   - Signing key from the secret store; key rotation supported through `kid`.
4. **Cashier PIN login:** only on a terminal registered to the branch (terminal key + device secret),
   4 to 6 digit PIN hashed like a password, lockout after 5 wrong attempts, short session.
5. **Policies:** owners and tenant admins must use two-factor; Super Admin always. Password rules follow the
   framework defaults with a minimum length of 10. Lockout after 5 failed attempts for 15 minutes.
6. **Audit:** sign-in, failed sign-in, lockout, password change, 2FA change, role/permission change and
   session revocation are written to `audit` in the same transaction.
7. **Later:** external sign-in (Microsoft Entra ID, Google) for the Enterprise plan, linked to existing
   user records through `identity.UserLogin`.

## Alternatives considered

- External provider for everyone (Auth0, Entra External ID): no login code to own, but per-user cost,
  user data outside our database, and no good fit for till PIN login.
- Self-hosted Keycloak: free, but another critical system to run, patch and back up.

## Consequences

- We own the store code and token handling: it gets a dedicated security review and tests for lockout,
  token rotation, reuse detection and cross-tenant sign-in.
- Rate limits on sign-in endpoints are stricter than the global limit (ADR 0012).
