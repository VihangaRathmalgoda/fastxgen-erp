using FastXGen.Erp.Api.Errors;
using FastXGen.Erp.Application.Common.Validation;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FastXGen.Erp.Api.Validation;

/// <summary>
/// Runs the FluentValidation validator for every complex action argument (body or query object)
/// before the action executes. Fails closed: an argument type without a validator is a server error,
/// so an unvalidated request can never reach the application (ADR 0012).
/// Simple route values (ids) are protected by route constraints such as <c>{id:long:min(1)}</c>.
/// </summary>
internal sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var errors = new Dictionary<string, string[]>(StringComparer.Ordinal);

        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null || !NeedsValidator(argument.GetType()))
            {
                continue;
            }

            var validator = ResolveValidator(context.HttpContext.RequestServices, argument.GetType());
            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument), context.HttpContext.RequestAborted);

            foreach (var (field, messages) in result.Errors.ToErrorDictionary())
            {
                errors[field] = messages;
            }
        }

        if (errors.Count > 0)
        {
            context.Result = ProblemResponses.ValidationResult(context.HttpContext, errors);
            return;
        }

        await next();
    }

    private static bool NeedsValidator(Type type) => type.IsClass && type != typeof(string);

    private static IValidator ResolveValidator(IServiceProvider services, Type argumentType)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);
        return services.GetService(validatorType) as IValidator
            ?? throw new InvalidOperationException(
                $"No validator is registered for {argumentType.Name}. Every request type needs one (ADR 0012).");
    }
}
