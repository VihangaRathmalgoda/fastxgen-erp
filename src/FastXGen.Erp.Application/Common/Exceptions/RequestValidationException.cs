namespace FastXGen.Erp.Application.Common.Exceptions;

/// <summary>Input failed validation. Carries the messages per field; returned as 400 ValidationProblemDetails.</summary>
public sealed class RequestValidationException(IReadOnlyDictionary<string, string[]> errors)
    : AppException("validation_failed", "One or more fields are invalid.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}
