using Microsoft.Data.SqlClient;

namespace FastXGen.Erp.Persistence.Sql;

internal interface ISqlConnectionFactory
{
    /// <summary>Opens a connection with the current tenant and user stamped into SESSION_CONTEXT.</summary>
    Task<SqlConnection> OpenAsync(CancellationToken cancellationToken);
}
