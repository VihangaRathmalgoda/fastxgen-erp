/*
    Readiness probe used by the API health check. Reads no tenant data.
*/
CREATE PROCEDURE [core].[usp_Health_Get]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        CAST(N'ok' AS NVARCHAR(10)) AS [Status],
        SYSUTCDATETIME()            AS [DatabaseTimeUtc];
END;
