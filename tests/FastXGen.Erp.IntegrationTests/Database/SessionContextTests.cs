using Microsoft.Data.SqlClient;

namespace FastXGen.Erp.IntegrationTests.Database;

/// <summary>
/// ADR 0004 relies on SESSION_CONTEXT never leaking between requests through the connection pool.
/// </summary>
[Trait("Category", TestDatabase.Category)]
public sealed class SessionContextTests
{
    [Fact]
    public async Task Pooled_connection_does_not_carry_the_previous_tenant()
    {
        // A pool of one connection forces the second open to reuse the first physical connection.
        var connectionString = new SqlConnectionStringBuilder(TestDatabase.ConnectionString())
        {
            MaxPoolSize = 1,
            ApplicationName = "fastxgen-session-context-test",
        }.ConnectionString;

        await using (var first = await TestDatabase.ConnectionFactoryFor(42, 7, connectionString).OpenAsync(Ct))
        {
            Assert.Equal(42, await ReadTenantAsync(first));
        }

        await using var second = await TestDatabase.ConnectionFactoryFor(null, null, connectionString).OpenAsync(Ct);
        Assert.Null(await ReadTenantAsync(second));
    }

    [Fact]
    public async Task Tenant_in_session_context_cannot_be_overwritten()
    {
        await using var connection = await TestDatabase
            .ConnectionFactoryFor(42, 7, TestDatabase.ConnectionString())
            .OpenAsync(Ct);

        await using var command = new SqlCommand("EXEC sys.sp_set_session_context N'TenantId', 99;", connection);

        await Assert.ThrowsAsync<SqlException>(() => command.ExecuteNonQueryAsync(Ct));
        Assert.Equal(42, await ReadTenantAsync(connection));
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    private static async Task<int?> ReadTenantAsync(SqlConnection connection)
    {
        await using var command = new SqlCommand("SELECT CAST(SESSION_CONTEXT(N'TenantId') AS INT);", connection);
        var value = await command.ExecuteScalarAsync(Ct);
        return value is DBNull or null ? null : (int)value;
    }
}
