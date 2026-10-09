using FastXGen.Erp.Contracts.Common;
using FastXGen.Erp.Contracts.Notifications;

namespace FastXGen.Erp.Application.Abstractions.Notifications;

/// <summary>
/// In-app notifications for users of the current tenant (ADR 0013).
/// Reads and updates act only on the current user's own notifications.
/// </summary>
public interface INotificationStore
{
    Task<long> CreateAsync(NotificationDraft draft, CancellationToken cancellationToken);

    Task<PagedResponse<NotificationResponse>> ListMineAsync(
        PageRequest page, bool unreadOnly, CancellationToken cancellationToken);

    Task<int> CountMyUnreadAsync(CancellationToken cancellationToken);

    Task MarkReadAsync(long notificationId, CancellationToken cancellationToken);
}
