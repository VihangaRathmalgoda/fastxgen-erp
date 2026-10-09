namespace FastXGen.Erp.Persistence.Sql;

/// <summary>
/// The only way repositories reach the database. Every call is a stored procedure,
/// and each procedure owns its own transaction.
/// </summary>
internal interface ISqlExecutor
{
    Task<IReadOnlyList<T>> QueryAsync<T>(
        StoredProcedureName procedure, object? parameters, CancellationToken cancellationToken);

    Task<T?> QuerySingleOrDefaultAsync<T>(
        StoredProcedureName procedure, object? parameters, CancellationToken cancellationToken);

    Task<int> ExecuteAsync(
        StoredProcedureName procedure, object? parameters, CancellationToken cancellationToken);
}
