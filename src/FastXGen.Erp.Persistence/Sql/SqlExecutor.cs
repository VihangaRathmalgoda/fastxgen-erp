using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace FastXGen.Erp.Persistence.Sql;

internal sealed class SqlExecutor(ISqlConnectionFactory connectionFactory, IOptions<DatabaseOptions> options)
    : ISqlExecutor
{
    public Task<IReadOnlyList<T>> QueryAsync<T>(
        StoredProcedureName procedure, object? parameters, CancellationToken cancellationToken) =>
        RunAsync<IReadOnlyList<T>>(
            async (connection, command) => (await connection.QueryAsync<T>(command)).AsList(),
            procedure, parameters, cancellationToken);

    public Task<T?> QuerySingleOrDefaultAsync<T>(
        StoredProcedureName procedure, object? parameters, CancellationToken cancellationToken) =>
        RunAsync(
            (connection, command) => connection.QuerySingleOrDefaultAsync<T>(command),
            procedure, parameters, cancellationToken);

    public Task<int> ExecuteAsync(
        StoredProcedureName procedure, object? parameters, CancellationToken cancellationToken) =>
        RunAsync(
            (connection, command) => connection.ExecuteAsync(command),
            procedure, parameters, cancellationToken);

    private async Task<TResult> RunAsync<TResult>(
        Func<SqlConnection, CommandDefinition, Task<TResult>> action,
        StoredProcedureName procedure,
        object? parameters,
        CancellationToken cancellationToken)
    {
        var command = new CommandDefinition(
            procedure.Value,
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: options.Value.CommandTimeoutSeconds,
            cancellationToken: cancellationToken);

        await using var connection = await connectionFactory.OpenAsync(cancellationToken);
        try
        {
            return await action(connection, command);
        }
        catch (SqlException ex) when (SqlErrorTranslator.Translate(ex.Number, ex.Message) is { } translated)
        {
            throw translated;
        }
    }
}
