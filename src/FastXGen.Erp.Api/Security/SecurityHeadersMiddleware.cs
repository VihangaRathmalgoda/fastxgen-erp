namespace FastXGen.Erp.Api.Security;

/// <summary>
/// Adds defensive response headers. The API serves JSON only, so the content security policy blocks
/// everything, and API responses are never cached because they hold tenant data.
/// </summary>
public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public Task InvokeAsync(HttpContext context)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            headers.XContentTypeOptions = "nosniff";
            headers.XFrameOptions = "DENY";
            headers.ContentSecurityPolicy = "default-src 'none'; frame-ancestors 'none'";
            headers["Referrer-Policy"] = "no-referrer";
            headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

            if (context.Request.Path.StartsWithSegments("/" + ApiRoutes.V1, StringComparison.OrdinalIgnoreCase))
            {
                headers.CacheControl = "no-store";
            }

            return Task.CompletedTask;
        });

        return next(context);
    }
}
