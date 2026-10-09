using FastXGen.Erp.Application.Abstractions.Notifications;
using FastXGen.Erp.Contracts.Common;
using FastXGen.Erp.Contracts.Notifications;
using FastXGen.Erp.Persistence.Sql;

namespace FastXGen.Erp.Persistence.Repositories.Notifications;

internal sealed class NotificationStore(ISqlExecutor sql) : INotificationStore
{
    private static readonly StoredProcedureName Create = StoredProcedureName.From("notify.usp_Notification_Create");
    private static readonly StoredProcedureName List = StoredProcedureName.From("notify.usp_Notification_List");
    private static readonly StoredProcedureName CountUnread = StoredProcedureName.From("notify.usp_Notification_CountUnread");
    private static readonly StoredProcedureName MarkRead = StoredProcedureName.From("notify.usp_Notification_MarkRead");

    public async Task<long> CreateAsync(NotificationDraft draft, CancellationToken cancellationToken)
    {
        var parameters = new
        {
            draft.RecipientUserId,
            draft.Category,
            Severity = (byte)draft.Severity,
            draft.Title,
            draft.Body,
            draft.Link,
        };

        return await sql.QuerySingleOrDefaultAsync<long>(Create, parameters, cancellationToken);
    }

    public async Task<PagedResponse<NotificationResponse>> ListMineAsync(
        PageRequest page, bool unreadOnly, CancellationToken cancellationToken)
    {
        var rows = await sql.QueryAsync<NotificationRow>(
            List, new { UnreadOnly = unreadOnly, page.Page, page.PageSize }, cancellationToken);

        var items = rows.Select(row => row.ToResponse()).ToList();
        var totalCount = rows.Count > 0 ? rows[0].TotalCount : 0;

        return new PagedResponse<NotificationResponse>(items, page.Page, page.PageSize, totalCount);
    }

    public Task<int> CountMyUnreadAsync(CancellationToken cancellationToken) =>
        sql.QuerySingleOrDefaultAsync<int>(CountUnread, null, cancellationToken);

    public Task MarkReadAsync(long notificationId, CancellationToken cancellationToken) =>
        sql.ExecuteAsync(MarkRead, new { NotificationId = notificationId }, cancellationToken);

    private sealed record NotificationRow(
        long NotificationId,
        string Category,
        byte Severity,
        string Title,
        string? Body,
        string? Link,
        DateTime CreatedAtUtc,
        DateTime? ReadAtUtc,
        int TotalCount)
    {
        public NotificationResponse ToResponse() => new(
            NotificationId,
            Category,
            (MessageSeverity)Severity,
            Title,
            Body,
            Link,
            AsUtc(CreatedAtUtc),
            ReadAtUtc is { } readAt ? AsUtc(readAt) : null);

        private static DateTimeOffset AsUtc(DateTime value) =>
            new(DateTime.SpecifyKind(value, DateTimeKind.Utc));
    }
}
