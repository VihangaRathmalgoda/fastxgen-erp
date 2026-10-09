using FastXGen.Erp.Contracts.Common;

namespace FastXGen.Erp.Application.Abstractions.Notifications;

/// <summary>
/// A notification to store for one user of the current tenant.
/// <c>Category</c> groups notifications by module, for example "inventory.low_stock".
/// <c>Link</c> optionally points inside the application, for example "/inventory/products/42".
/// </summary>
public sealed record NotificationDraft(
    Guid RecipientUserId,
    string Category,
    MessageSeverity Severity,
    string Title,
    string? Body,
    string? Link);
