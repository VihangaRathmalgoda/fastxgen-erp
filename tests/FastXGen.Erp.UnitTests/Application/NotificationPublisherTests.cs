using FastXGen.Erp.Application.Abstractions.Notifications;
using FastXGen.Erp.Application.Common.Exceptions;
using FastXGen.Erp.Application.Features.Notifications;
using FastXGen.Erp.Contracts.Common;
using FastXGen.Erp.Contracts.Notifications;

namespace FastXGen.Erp.UnitTests.Application;

public sealed class NotificationPublisherTests
{
    private readonly RecordingStore _store = new();
    private readonly NotificationPublisher _publisher;

    public NotificationPublisherTests()
    {
        _publisher = new NotificationPublisher(new NotificationDraftValidator(), _store);
    }

    [Fact]
    public async Task Valid_draft_is_stored_and_its_id_returned()
    {
        var draft = Draft();

        var id = await _publisher.PublishAsync(draft, TestContext.Current.CancellationToken);

        Assert.Equal(RecordingStore.NextId, id);
        Assert.Same(draft, _store.Created.Single());
    }

    [Fact]
    public async Task Invalid_draft_is_rejected_before_reaching_the_database()
    {
        var draft = Draft() with { Title = "<img src=x onerror=alert(1)>", Link = "javascript:alert(1)" };

        var exception = await Assert.ThrowsAsync<RequestValidationException>(
            () => _publisher.PublishAsync(draft, TestContext.Current.CancellationToken));

        Assert.Contains(nameof(NotificationDraft.Title), exception.Errors.Keys);
        Assert.Contains(nameof(NotificationDraft.Link), exception.Errors.Keys);
        Assert.Empty(_store.Created);
    }

    [Fact]
    public async Task Empty_recipient_key_is_rejected()
    {
        var draft = Draft() with { RecipientUserId = Guid.Empty };

        var exception = await Assert.ThrowsAsync<RequestValidationException>(
            () => _publisher.PublishAsync(draft, TestContext.Current.CancellationToken));

        Assert.Contains(nameof(NotificationDraft.RecipientUserId), exception.Errors.Keys);
    }

    [Fact]
    public async Task Unknown_severity_value_is_rejected()
    {
        var draft = Draft() with { Severity = (MessageSeverity)9 };

        await Assert.ThrowsAsync<RequestValidationException>(
            () => _publisher.PublishAsync(draft, TestContext.Current.CancellationToken));
    }

    private static NotificationDraft Draft() => new(
        RecipientUserId: Guid.Parse("0d4f6a2e-1c3b-4e5f-8a9b-7c6d5e4f3a2b"),
        Category: "inventory.low_stock",
        Severity: MessageSeverity.Warning,
        Title: "Low stock: Coca-Cola 500ml",
        Body: "Only 4 left at Main Street.",
        Link: "/inventory/products/42");

    private sealed class RecordingStore : INotificationStore
    {
        public const long NextId = 7;

        public List<NotificationDraft> Created { get; } = [];

        public Task<long> CreateAsync(NotificationDraft draft, CancellationToken cancellationToken)
        {
            Created.Add(draft);
            return Task.FromResult(NextId);
        }

        public Task<PagedResponse<NotificationResponse>> ListMineAsync(
            PageRequest page, bool unreadOnly, CancellationToken cancellationToken) =>
            throw new NotSupportedException();

        public Task<int> CountMyUnreadAsync(CancellationToken cancellationToken) => throw new NotSupportedException();

        public Task MarkReadAsync(long notificationId, CancellationToken cancellationToken) =>
            throw new NotSupportedException();
    }
}
