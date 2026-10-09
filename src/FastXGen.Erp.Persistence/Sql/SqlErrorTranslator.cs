using FastXGen.Erp.Application.Common.Exceptions;

namespace FastXGen.Erp.Persistence.Sql;

/// <summary>
/// Maps SQL Server error numbers to application exceptions.
/// Procedures raise expected failures with THROW using the numbers below
/// (see docs/standards/database-standards.md).
/// </summary>
internal static class SqlErrorTranslator
{
    public const int Forbidden = 50403;
    public const int NotFound = 50404;
    public const int Conflict = 50409;
    public const int BusinessRule = 50422;

    private const int UniqueIndexViolation = 2601;
    private const int UniqueConstraintViolation = 2627;

    /// <summary>Returns the matching application exception, or null when the error is unexpected.</summary>
    public static AppException? Translate(int errorNumber, string message) => errorNumber switch
    {
        Forbidden => new ForbiddenException("forbidden", message),
        NotFound => new NotFoundException("not_found", message),
        Conflict => new ConflictException("conflict", message),
        BusinessRule => new BusinessRuleException("business_rule", message),
        UniqueIndexViolation or UniqueConstraintViolation =>
            new ConflictException("duplicate", "A record with the same unique value already exists."),
        _ => null,
    };
}
