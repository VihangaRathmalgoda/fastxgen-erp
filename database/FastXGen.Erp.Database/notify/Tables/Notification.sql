/*
    In-app notifications (ADR 0013). One row per recipient.
    Severity: 0 Info, 1 Success, 2 Warning, 3 Error (FastXGen.Erp.Contracts.Common.MessageSeverity).
*/
CREATE TABLE [notify].[Notification]
(
    [TenantId]        INT             NOT NULL,
    [NotificationId]  BIGINT          IDENTITY (1, 1) NOT NULL,
    [RecipientUserId] BIGINT          NOT NULL,
    [Category]        NVARCHAR (50)   NOT NULL,
    [Severity]        TINYINT         NOT NULL,
    [Title]           NVARCHAR (200)  NOT NULL,
    [Body]            NVARCHAR (1000) NULL,
    [Link]            NVARCHAR (400)  NULL,
    [CreatedAtUtc]    DATETIME2 (3)   NOT NULL CONSTRAINT [DF_Notification_CreatedAtUtc] DEFAULT (SYSUTCDATETIME()),
    [CreatedByUserId] BIGINT          NULL,
    [ReadAtUtc]       DATETIME2 (3)   NULL,
    CONSTRAINT [PK_Notification] PRIMARY KEY CLUSTERED ([TenantId], [NotificationId]),
    CONSTRAINT [CK_Notification_Severity] CHECK ([Severity] BETWEEN 0 AND 3),
    CONSTRAINT [CK_Notification_RecipientUserId] CHECK ([RecipientUserId] > 0)
);
GO

CREATE NONCLUSTERED INDEX [IX_Notification_RecipientUserId_CreatedAtUtc]
    ON [notify].[Notification] ([TenantId], [RecipientUserId], [CreatedAtUtc] DESC)
    INCLUDE ([ReadAtUtc]);
