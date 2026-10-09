using System.Threading.RateLimiting;
using FastXGen.Erp.Api.Errors;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;

namespace FastXGen.Erp.Api.Security;

internal static class RateLimitingSetup
{
    public static IServiceCollection AddApiRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<RateLimitSettings>().Bind(configuration.GetSection(RateLimitSettings.SectionName));

        services.AddRateLimiter(_ => { });
        services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimitSettings>>((options, settings) =>
            {
                var permits = settings.Value.PermitsPerMinute;

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    context.Request.Path.StartsWithSegments("/health")
                        ? RateLimitPartition.GetNoLimiter("health")
                        : RateLimitPartition.GetFixedWindowLimiter(PartitionKey(context), _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = permits,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                        }));

                options.OnRejected = (context, cancellationToken) => new ValueTask(ProblemResponses.WriteAsync(
                    context.HttpContext,
                    ProblemResponses.Create(
                        context.HttpContext,
                        StatusCodes.Status429TooManyRequests,
                        "rate_limited",
                        "Too many requests. Please wait a moment and try again."),
                    cancellationToken));
            });

        return services;
    }

    private static string PartitionKey(HttpContext context) =>
        context.User.Identity?.IsAuthenticated == true
            ? "user:" + context.User.Identity.Name
            : "ip:" + context.Connection.RemoteIpAddress;
}
