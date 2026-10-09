namespace FastXGen.Erp.Application.Common.Exceptions;

/// <summary>
/// Base type for expected failures that map to a specific HTTP problem response.
/// Unexpected failures stay ordinary exceptions and surface as 500.
/// </summary>
public abstract class AppException(string errorCode, string message) : Exception(message)
{
    /// <summary>Stable, machine-readable code, for example "sales.invoice.not_found".</summary>
    public string ErrorCode { get; } = errorCode;
}
