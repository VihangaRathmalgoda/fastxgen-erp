using FastXGen.Erp.Contracts.Common;

namespace FastXGen.Erp.Contracts.Notifications;

public sealed record NotificationResponse(
    long NotificationId,
    string Category,
    MessageSeverity Severity,
    string Title,
    string? Body,
    string? Link,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset? ReadAtUtc);
