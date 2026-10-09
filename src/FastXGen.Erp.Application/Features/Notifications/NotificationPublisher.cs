using FastXGen.Erp.Application.Abstractions.Notifications;
using FastXGen.Erp.Application.Common.Validation;
using FluentValidation;

namespace FastXGen.Erp.Application.Features.Notifications;

internal sealed class NotificationPublisher(IValidator<NotificationDraft> validator, INotificationStore store)
    : INotificationPublisher
{
    public async Task<long> PublishAsync(NotificationDraft draft, CancellationToken cancellationToken)
    {
        await validator.ValidateOrThrowAsync(draft, cancellationToken);
        return await store.CreateAsync(draft, cancellationToken);
    }
}
