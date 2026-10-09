using FastXGen.Erp.Application.Common.Exceptions;
using FluentValidation;
using FluentValidation.Results;

namespace FastXGen.Erp.Application.Common.Validation;

public static class ValidatorExtensions
{
    /// <summary>Validates input that did not come through an HTTP request (internal calls, jobs, imports).</summary>
    public static async Task ValidateOrThrowAsync<T>(
        this IValidator<T> validator, T instance, CancellationToken cancellationToken)
    {
        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (!result.IsValid)
        {
            throw new RequestValidationException(result.Errors.ToErrorDictionary());
        }
    }

    public static IReadOnlyDictionary<string, string[]> ToErrorDictionary(this IEnumerable<ValidationFailure> failures) =>
        failures
            .GroupBy(failure => failure.PropertyName, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.Select(failure => failure.ErrorMessage).ToArray());
}
