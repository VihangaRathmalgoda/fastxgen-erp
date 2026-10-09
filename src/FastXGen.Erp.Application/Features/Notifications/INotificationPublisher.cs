using FastXGen.Erp.Application.Abstractions.Notifications;

namespace FastXGen.Erp.Application.Features.Notifications;

/// <summary>
/// How any module notifies a user, for example when an approval is waiting or stock runs low (ADR 0013).
/// </summary>
public interface INotificationPublisher
{
    Task<long> PublishAsync(NotificationDraft draft, CancellationToken cancellationToken);
}
