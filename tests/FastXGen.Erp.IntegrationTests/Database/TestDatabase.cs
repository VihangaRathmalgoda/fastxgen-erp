using FastXGen.Erp.Application.Abstractions.Tenancy;
using FastXGen.Erp.Persistence.Sql;
using Microsoft.Extensions.Options;

namespace FastXGen.Erp.IntegrationTests.Database;

/// <summary>
/// Access to a deployed test database. Tests that use it carry [Trait("Category", "Database")]
/// and are skipped when no connection string is configured (ADR 0011).
/// </summary>
internal static class TestDatabase
{
    public const string Category = "Database";
    private const string ConnectionStringVariable = "Database__ConnectionString";

    public static string ConnectionString()
    {
        var value = Environment.GetEnvironmentVariable(ConnectionStringVariable);
        if (string.IsNullOrWhiteSpace(value))
        {
            Assert.Skip($"Set {ConnectionStringVariable} to run database tests.");
        }

        return value;
    }

    /// <summary>A SQL executor acting as the given tenant and user, exactly as the API would.</summary>
    public static ISqlExecutor ExecutorFor(int? tenantId, long? userId, string? connectionString = null)
    {
        var options = Options.Create(new DatabaseOptions { ConnectionString = connectionString ?? ConnectionString() });
        var factory = new SqlConnectionFactory(options, new FixedTenantContext(tenantId, userId));
        return new SqlExecutor(factory, options);
    }

    public static SqlConnectionFactory ConnectionFactoryFor(int? tenantId, long? userId, string connectionString) =>
        new(Options.Create(new DatabaseOptions { ConnectionString = connectionString }), new FixedTenantContext(tenantId, userId));

    /// <summary>A tenant id no other test run uses, so tests never see each other's rows.</summary>
    public static int NewTenantId() => Random.Shared.Next(1_000_000, int.MaxValue);

    private sealed record FixedTenantContext(int? TenantId, long? UserId) : ITenantContext;
}
