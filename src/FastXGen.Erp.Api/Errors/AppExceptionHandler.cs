using FastXGen.Erp.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FastXGen.Erp.Api.Errors;

/// <summary>
/// Turns expected application failures into ProblemDetails responses.
/// Anything else falls through to the default handler and becomes a 500 without internal details.
/// </summary>
internal sealed class AppExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not AppException appException)
        {
            return false;
        }

        ProblemDetails problem = appException switch
        {
            RequestValidationException validation => ProblemResponses.CreateValidation(httpContext, validation.Errors),
            NotFoundException => Create(StatusCodes.Status404NotFound),
            ConflictException => Create(StatusCodes.Status409Conflict),
            ForbiddenException => Create(StatusCodes.Status403Forbidden),
            BusinessRuleException => Create(StatusCodes.Status422UnprocessableEntity),
            _ => Create(StatusCodes.Status400BadRequest),
        };

        await ProblemResponses.WriteAsync(httpContext, problem, cancellationToken);
        return true;

        ProblemDetails Create(int status) =>
            ProblemResponses.Create(httpContext, status, appException.ErrorCode, appException.Message);
    }
}
