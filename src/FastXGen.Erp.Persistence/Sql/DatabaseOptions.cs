namespace FastXGen.Erp.Persistence.Sql;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    /// <summary>Supplied by environment variables or a secret store; never committed.</summary>
    public string ConnectionString { get; set; } = string.Empty;

    public int CommandTimeoutSeconds { get; set; } = 30;
}
