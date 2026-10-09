using System.Text.Json;
using FastXGen.Erp.Api.Middleware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;

namespace FastXGen.Erp.Api.Errors;

/// <summary>
/// Builds every error response the API returns, so clients always get the same shape (ADR 0013):
/// <c>{ type, title, status, detail, code, correlationId, errors? }</c>.
/// <c>code</c> is stable and machine-readable; <c>detail</c> is safe to show to the user.
/// </summary>
internal static class ProblemResponses
{
    public const string CodeKey = "code";
    public const string ValidationFailedCode = "validation_failed";
    private const string ProblemJson = "application/problem+json";

    /// <summary>Applied to every ProblemDetails the framework creates (404s, 500s, binding errors).</summary>
    public static void Customize(ProblemDetailsContext context)
    {
        var problem = context.ProblemDetails;
        problem.Extensions[CorrelationIdMiddleware.ItemKey] = context.HttpContext.Items[CorrelationIdMiddleware.ItemKey];
        problem.Extensions.TryAdd(CodeKey, DefaultCode(problem.Status ?? StatusCodes.Status500InternalServerError));
    }

    public static ProblemDetails Create(HttpContext httpContext, int status, string code, string detail)
    {
        var problem = Factory(httpContext).CreateProblemDetails(
            httpContext, status, title: ReasonPhrases.GetReasonPhrase(status), detail: detail);
        problem.Extensions[CodeKey] = code;
        return problem;
    }

    public static ValidationProblemDetails CreateValidation(
        HttpContext httpContext, IReadOnlyDictionary<string, string[]> errors)
    {
        var modelState = new ModelStateDictionary();
        foreach (var (field, messages) in errors)
        {
            foreach (var message in messages)
            {
                modelState.AddModelError(ToCamelCase(field), message);
            }
        }

        return CreateValidation(httpContext, modelState);
    }

    public static ValidationProblemDetails CreateValidation(HttpContext httpContext, ModelStateDictionary modelState)
    {
        var problem = Factory(httpContext).CreateValidationProblemDetails(
            httpContext,
            modelState,
            StatusCodes.Status400BadRequest,
            title: "Validation failed",
            detail: "One or more fields are invalid.");
        problem.Extensions[CodeKey] = ValidationFailedCode;
        return problem;
    }

    public static IActionResult ValidationResult(HttpContext httpContext, ModelStateDictionary modelState) =>
        new BadRequestObjectResult(CreateValidation(httpContext, modelState)) { ContentTypes = { ProblemJson } };

    public static IActionResult ValidationResult(HttpContext httpContext, IReadOnlyDictionary<string, string[]> errors) =>
        new BadRequestObjectResult(CreateValidation(httpContext, errors)) { ContentTypes = { ProblemJson } };

    public static Task WriteAsync(HttpContext httpContext, ProblemDetails problem, CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = problem.Status ?? StatusCodes.Status500InternalServerError;
        return httpContext.Response.WriteAsJsonAsync(
            problem, problem.GetType(), options: null, contentType: ProblemJson, cancellationToken: cancellationToken);
    }

    private static string DefaultCode(int status) => status switch
    {
        StatusCodes.Status400BadRequest => "bad_request",
        StatusCodes.Status401Unauthorized => "unauthorized",
        StatusCodes.Status403Forbidden => "forbidden",
        StatusCodes.Status404NotFound => "not_found",
        StatusCodes.Status405MethodNotAllowed => "method_not_allowed",
        StatusCodes.Status409Conflict => "conflict",
        StatusCodes.Status413PayloadTooLarge => "payload_too_large",
        StatusCodes.Status415UnsupportedMediaType => "unsupported_media_type",
        StatusCodes.Status422UnprocessableEntity => "business_rule",
        StatusCodes.Status429TooManyRequests => "rate_limited",
        _ => "internal_error",
    };

    private static ProblemDetailsFactory Factory(HttpContext httpContext) =>
        httpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();

    private static string ToCamelCase(string field) =>
        string.Join('.', field.Split('.').Select(JsonNamingPolicy.CamelCase.ConvertName));
}
