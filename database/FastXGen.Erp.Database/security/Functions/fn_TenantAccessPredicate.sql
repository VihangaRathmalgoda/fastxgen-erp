/*
    Row-Level Security predicate shared by every tenant-owned table.
    A row is visible (filter) and writable (block) only when its TenantId matches the
    TenantId the API stamped into SESSION_CONTEXT when it opened the connection.
    Every tenant-owned table gets a FILTER and a BLOCK predicate in
    security/SecurityPolicies/TenantIsolationPolicy.sql.
*/
CREATE FUNCTION [security].[fn_TenantAccessPredicate]
(
    @TenantId INT
)
RETURNS TABLE
WITH SCHEMABINDING
AS
RETURN
    SELECT 1 AS [AccessResult]
    WHERE @TenantId = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
