/*
    Stores a notification for one user of the current tenant. Input is validated in C#
    (NotificationDraftValidator); the table constraints are the last line of defence.
    CreatedByUserId is NULL when the system raised the notification.
*/
CREATE PROCEDURE [notify].[usp_Notification_Create]
    @RecipientUserId UNIQUEIDENTIFIER,
    @Category        NVARCHAR (50),
    @Severity        TINYINT,
    @Title           NVARCHAR (200),
    @Body            NVARCHAR (1000) = NULL,
    @Link            NVARCHAR (400)  = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TenantId INT              = CAST(SESSION_CONTEXT(N'TenantId') AS INT);
    DECLARE @UserId   UNIQUEIDENTIFIER = CAST(SESSION_CONTEXT(N'UserId') AS UNIQUEIDENTIFIER);

    IF @TenantId IS NULL
        THROW 50403, N'No tenant context.', 1;

    INSERT INTO [notify].[Notification]
        ([TenantId], [RecipientUserId], [Category], [Severity], [Title], [Body], [Link], [CreatedByUserId])
    VALUES
        (@TenantId, @RecipientUserId, @Category, @Severity, @Title, @Body, @Link, @UserId);

    SELECT CAST(SCOPE_IDENTITY() AS BIGINT) AS [NotificationId];
END;
