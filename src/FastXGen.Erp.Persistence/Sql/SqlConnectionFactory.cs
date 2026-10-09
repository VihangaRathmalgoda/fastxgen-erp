using System.Data;
using Dapper;
using FastXGen.Erp.Application.Abstractions.Tenancy;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FastXGen.Erp.Persistence.Sql;

internal sealed class SqlConnectionFactory(IOptions<DatabaseOptions> options, ITenantContext tenantContext)
    : ISqlConnectionFactory
{
    public async Task<SqlConnection> OpenAsync(CancellationToken cancellationToken)
    {
        var connection = new SqlConnection(options.Value.ConnectionString);
        try
        {
            await connection.OpenAsync(cancellationToken);
            await SetSessionValueAsync(connection, "TenantId", tenantContext.TenantId, cancellationToken);
            await SetSessionValueAsync(connection, "UserId", tenantContext.UserId, cancellationToken);
            return connection;
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    // read_only = 1 means nothing later on this connection, including procedures, can change the value.
    private static Task SetSessionValueAsync(
        SqlConnection connection, string key, object? value, CancellationToken cancellationToken)
    {
        if (value is null)
        {
            return Task.CompletedTask;
        }

        return connection.ExecuteAsync(new CommandDefinition(
            "sys.sp_set_session_context",
            new { key, value, read_only = 1 },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken));
    }
}
