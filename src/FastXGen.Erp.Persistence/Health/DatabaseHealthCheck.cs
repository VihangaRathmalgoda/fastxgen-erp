using FastXGen.Erp.Persistence.Sql;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace FastXGen.Erp.Persistence.Health;

internal sealed class DatabaseHealthCheck(ISqlExecutor sql) : IHealthCheck
{
    private static readonly StoredProcedureName HealthProcedure = StoredProcedureName.From("core.usp_Health_Get");

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await sql.QuerySingleOrDefaultAsync<DatabaseHealthRow>(HealthProcedure, null, cancellationToken);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database is not reachable.", ex);
        }
    }

    private sealed record DatabaseHealthRow(string Status, DateTime DatabaseTimeUtc);
}
