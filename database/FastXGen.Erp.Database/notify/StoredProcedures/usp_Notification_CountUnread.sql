/*
    Number of unread notifications for the current user (the badge on the bell icon).
*/
CREATE PROCEDURE [notify].[usp_Notification_CountUnread]
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TenantId INT              = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
    DECLARE @UserId   UNIQUEIDENTIFIER = CAST(SESSION_CONTEXT(N'UserId') AS UNIQUEIDENTIFIER);

    IF @TenantId IS NULL OR @UserId IS NULL
        THROW 50403, N'No tenant or user context.', 1;

    SELECT COUNT(*) AS [UnreadCount]
    FROM [notify].[Notification]
    WHERE [TenantId] = @TenantId
      AND [RecipientUserId] = @UserId
      AND [ReadAtUtc] IS NULL;
END;
