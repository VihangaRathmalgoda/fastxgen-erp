/*
    Row-Level Security for every tenant-owned table (ADR 0004).
    FILTER hides other tenants' rows from reads; BLOCK stops inserts and updates that would
    write a row for another tenant. Add BOTH lines for every new tenant-owned table.
*/
CREATE SECURITY POLICY [security].[TenantIsolationPolicy]
    ADD FILTER PREDICATE [security].[fn_TenantAccessPredicate]([TenantId]) ON [notify].[Notification],
    ADD BLOCK PREDICATE [security].[fn_TenantAccessPredicate]([TenantId]) ON [notify].[Notification]
WITH (STATE = ON, SCHEMABINDING = ON);
