using FastXGen.Erp.Application.Abstractions.Notifications;
using FastXGen.Erp.Application.Common.Exceptions;
using FastXGen.Erp.Contracts.Common;
using FastXGen.Erp.Persistence.Repositories.Notifications;

namespace FastXGen.Erp.IntegrationTests.Database;

[Trait("Category", TestDatabase.Category)]
public sealed class NotificationStoreTests
{
    private static readonly Guid UserA = Guid.NewGuid();
    private static readonly Guid UserB = Guid.NewGuid();

    private readonly int _tenant1 = TestDatabase.NewTenantId();
    private readonly int _tenant2 = TestDatabase.NewTenantId();

    [Fact]
    public async Task User_sees_own_notifications_newest_first()
    {
        var store = StoreFor(_tenant1, UserA);
        await store.CreateAsync(Draft("First"), Ct);
        await store.CreateAsync(Draft("Second"), Ct);

        var page = await store.ListMineAsync(new PageRequest(1, 50), unreadOnly: false, Ct);

        Assert.Equal(2, page.TotalCount);
        Assert.Equal(["Second", "First"], page.Items.Select(item => item.Title));
        Assert.Equal(MessageSeverity.Warning, page.Items[0].Severity);
    }

    [Fact]
    public async Task Another_tenant_cannot_read_the_notifications()
    {
        await StoreFor(_tenant1, UserA).CreateAsync(Draft("Tenant 1 only"), Ct);

        var otherTenant = await StoreFor(_tenant2, UserA).ListMineAsync(new PageRequest(1, 50), unreadOnly: false, Ct);

        Assert.Empty(otherTenant.Items);
    }

    [Fact]
    public async Task Another_tenant_cannot_mark_the_notification_read()
    {
        var id = await StoreFor(_tenant1, UserA).CreateAsync(Draft("Tenant 1 only"), Ct);

        await Assert.ThrowsAsync<NotFoundException>(() => StoreFor(_tenant2, UserA).MarkReadAsync(id, Ct));
        Assert.Equal(1, await StoreFor(_tenant1, UserA).CountMyUnreadAsync(Ct));
    }

    [Fact]
    public async Task Another_user_of_the_same_tenant_cannot_mark_the_notification_read()
    {
        var id = await StoreFor(_tenant1, UserA).CreateAsync(Draft("For user A"), Ct);

        await Assert.ThrowsAsync<NotFoundException>(() => StoreFor(_tenant1, UserB).MarkReadAsync(id, Ct));
    }

    [Fact]
    public async Task Marking_read_updates_the_unread_count_and_can_be_repeated()
    {
        var store = StoreFor(_tenant1, UserA);
        var id = await store.CreateAsync(Draft("Read me"), Ct);

        await store.MarkReadAsync(id, Ct);
        await store.MarkReadAsync(id, Ct);

        Assert.Equal(0, await store.CountMyUnreadAsync(Ct));
        Assert.Empty((await store.ListMineAsync(new PageRequest(1, 50), unreadOnly: true, Ct)).Items);
    }

    [Fact]
    public async Task Calls_without_tenant_context_are_refused()
    {
        await Assert.ThrowsAsync<ForbiddenException>(() => StoreFor(null, null).CreateAsync(Draft("No tenant"), Ct));
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static NotificationStore StoreFor(int? tenantId, Guid? userId) =>
        new(TestDatabase.ExecutorFor(tenantId, userId));

    private static NotificationDraft Draft(string title) => new(
        RecipientUserId: UserA,
        Category: "inventory.low_stock",
        Severity: MessageSeverity.Warning,
        Title: title,
        Body: null,
        Link: "/inventory/products/42");
}
