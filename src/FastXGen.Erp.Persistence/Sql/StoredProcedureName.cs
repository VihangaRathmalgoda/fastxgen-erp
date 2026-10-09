using System.Text.RegularExpressions;

namespace FastXGen.Erp.Persistence.Sql;

/// <summary>
/// A validated stored procedure name in the form <c>schema.usp_Entity_Action</c>,
/// for example <c>sales.usp_Invoice_Create</c>. Free-form SQL text cannot reach the executor.
/// </summary>
internal readonly partial record struct StoredProcedureName
{
    private StoredProcedureName(string value) => Value = value;

    public string Value { get; }

    public static StoredProcedureName From(string value)
    {
        if (!NamePattern().IsMatch(value))
        {
            throw new ArgumentException(
                $"'{value}' is not a valid procedure name. Expected 'schema.usp_Entity_Action'.", nameof(value));
        }

        return new StoredProcedureName(value);
    }

    public override string ToString() => Value;

    [GeneratedRegex(@"^[a-z]+\.usp_[A-Z][A-Za-z0-9]*_[A-Z][A-Za-z0-9]*$")]
    private static partial Regex NamePattern();
}
