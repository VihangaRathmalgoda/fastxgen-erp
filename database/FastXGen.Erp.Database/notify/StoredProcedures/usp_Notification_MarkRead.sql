/*
    Marks one of the current user's notifications as read. Repeating the call is harmless.
    Another user's or another tenant's notification is reported as not found.
*/
CREATE PROCEDURE [notify].[usp_Notification_MarkRead]
    @NotificationId BIGINT
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TenantId INT              = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
    DECLARE @UserId   UNIQUEIDENTIFIER = CAST(SESSION_CONTEXT(N'UserId') AS UNIQUEIDENTIFIER);

    IF @TenantId IS NULL OR @UserId IS NULL
        THROW 50403, N'No tenant or user context.', 1;

    UPDATE [notify].[Notification]
    SET    [ReadAtUtc] = COALESCE([ReadAtUtc], SYSUTCDATETIME())
    WHERE  [TenantId] = @TenantId
      AND  [NotificationId] = @NotificationId
      AND  [RecipientUserId] = @UserId;

    IF @@ROWCOUNT = 0
        THROW 50404, N'Notification not found.', 1;
END;
