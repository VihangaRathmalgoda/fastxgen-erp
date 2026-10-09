using System.Text.RegularExpressions;
using FluentValidation;

namespace FastXGen.Erp.Application.Common.Validation;

/// <summary>
/// Reusable input rules. Every string, number and id in a request passes through one of these,
/// so the same field type is validated the same way everywhere (ADR 0012).
/// </summary>
public static partial class ValidationRules
{
    public const int MaxPageSize = 200;
    public const decimal MaxAmount = 99_999_999_999_999.9999m;

    /// <summary>
    /// Free text shown to people (names, descriptions, notes): length-limited, and free of control
    /// characters, angle brackets and bidirectional-override characters used for spoofing.
    /// Combine with NotEmpty() when required.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> PlainText<T>(this IRuleBuilder<T, string?> rule, int maxLength) =>
        rule.MaximumLength(maxLength)
            .Must(value => value is null || !UnsafeTextPattern().IsMatch(value))
            .WithErrorCode("validation.unsafe_text")
            .WithMessage("'{PropertyName}' contains characters that are not allowed.");

    /// <summary>Codes such as SKU, branch code or document prefix: letters, digits, dot, dash, underscore.</summary>
    public static IRuleBuilderOptions<T, string?> Code<T>(this IRuleBuilder<T, string?> rule, int maxLength) =>
        rule.MaximumLength(maxLength)
            .Must(value => value is null || CodePattern().IsMatch(value))
            .WithErrorCode("validation.invalid_code")
            .WithMessage("'{PropertyName}' may contain only letters, digits, '.', '-' and '_'.");

    /// <summary>Non-negative money amount that fits DECIMAL(18,4).</summary>
    public static IRuleBuilderOptions<T, decimal> Money<T>(this IRuleBuilder<T, decimal> rule) =>
        rule.InclusiveBetween(0m, MaxAmount)
            .PrecisionScale(18, 4, ignoreTrailingZeros: true);

    /// <summary>Positive quantity that fits DECIMAL(18,4).</summary>
    public static IRuleBuilderOptions<T, decimal> Quantity<T>(this IRuleBuilder<T, decimal> rule) =>
        rule.GreaterThan(0m)
            .LessThanOrEqualTo(MaxAmount)
            .PrecisionScale(18, 4, ignoreTrailingZeros: true);

    /// <summary>Percentage from 0 to 100 that fits DECIMAL(9,6).</summary>
    public static IRuleBuilderOptions<T, decimal> Percentage<T>(this IRuleBuilder<T, decimal> rule) =>
        rule.InclusiveBetween(0m, 100m)
            .PrecisionScale(9, 6, ignoreTrailingZeros: true);

    /// <summary>Identifier of an existing record. Whether it exists and is visible is checked by the procedure.</summary>
    public static IRuleBuilderOptions<T, long> EntityId<T>(this IRuleBuilder<T, long> rule) =>
        rule.GreaterThan(0L);

    public static IRuleBuilderOptions<T, string?> Email<T>(this IRuleBuilder<T, string?> rule) =>
        rule.MaximumLength(254)
            .EmailAddress();

    /// <summary>
    /// A link inside the application (for example "/sales/invoices/42"). Absolute URLs and schemes such as
    /// "javascript:" are rejected, which prevents open redirects and script injection through links.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> AppLink<T>(this IRuleBuilder<T, string?> rule) =>
        rule.MaximumLength(400)
            .Must(value => value is null || AppLinkPattern().IsMatch(value))
            .WithErrorCode("validation.invalid_link")
            .WithMessage("'{PropertyName}' must be a link inside the application, starting with '/'.");

    [GeneratedRegex(@"[\u0000-\u0008\u000B\u000C\u000E-\u001F\u007F<>‪-‮⁦-⁩]")]
    private static partial Regex UnsafeTextPattern();

    [GeneratedRegex(@"^[A-Za-z0-9][A-Za-z0-9._-]*$")]
    private static partial Regex CodePattern();

    [GeneratedRegex(@"^/(?!/)[A-Za-z0-9/_\-.?=&%]*$")]
    private static partial Regex AppLinkPattern();
}
