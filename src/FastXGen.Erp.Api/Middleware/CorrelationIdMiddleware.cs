using System.Text.RegularExpressions;

namespace FastXGen.Erp.Api.Middleware;

/// <summary>
/// Gives every request a correlation id: taken from the X-Correlation-Id header when it is safe,
/// otherwise generated. Echoed in the response, the log scope and every ProblemDetails body.
/// </summary>
public sealed partial class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string HeaderName = "X-Correlation-Id";
    public const string ItemKey = "correlationId";

    public async Task InvokeAsync(HttpContext context)
    {
        var incoming = context.Request.Headers[HeaderName].ToString();
        var correlationId = SafeId().IsMatch(incoming) ? incoming : Guid.NewGuid().ToString("N");

        context.Items[ItemKey] = correlationId;
        context.Response.Headers[HeaderName] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object> { [ItemKey] = correlationId }))
        {
            await next(context);
        }
    }

    [GeneratedRegex("^[A-Za-z0-9-]{8,64}$")]
    private static partial Regex SafeId();
}
