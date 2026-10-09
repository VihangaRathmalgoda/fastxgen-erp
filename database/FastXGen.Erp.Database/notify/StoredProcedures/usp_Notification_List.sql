/*
    The current user's notifications, newest first, paged.
*/
CREATE PROCEDURE [notify].[usp_Notification_List]
    @UnreadOnly BIT = 0,
    @Page       INT = 1,
    @PageSize   INT = 50
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @TenantId INT              = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
    DECLARE @UserId   UNIQUEIDENTIFIER = CAST(SESSION_CONTEXT(N'UserId') AS UNIQUEIDENTIFIER);

    IF @TenantId IS NULL OR @UserId IS NULL
        THROW 50403, N'No tenant or user context.', 1;

    IF @Page < 1 OR @PageSize NOT BETWEEN 1 AND 200
        THROW 50422, N'Page must be 1 or more and page size between 1 and 200.', 1;

    SELECT
        [NotificationId],
        [Category],
        [Severity],
        [Title],
        [Body],
        [Link],
        [CreatedAtUtc],
        [ReadAtUtc],
        COUNT(*) OVER () AS [TotalCount]
    FROM [notify].[Notification]
    WHERE [TenantId] = @TenantId
      AND [RecipientUserId] = @UserId
      AND (@UnreadOnly = 0 OR [ReadAtUtc] IS NULL)
    ORDER BY [CreatedAtUtc] DESC, [NotificationId] DESC
    OFFSET (@Page - 1) * @PageSize ROWS FETCH NEXT @PageSize ROWS ONLY;
END;
