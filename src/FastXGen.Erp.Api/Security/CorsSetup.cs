namespace FastXGen.Erp.Api.Security;

internal static class CorsSetup
{
    public const string PolicyName = "WebClients";

    public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(CorsSettings.SectionName).Get<CorsSettings>() ?? new CorsSettings();

        services.AddCors(options => options.AddPolicy(PolicyName, policy => policy
            .WithOrigins(settings.AllowedOrigins)
            .WithMethods("GET", "POST", "PUT", "PATCH", "DELETE")
            .WithHeaders("Content-Type", "Authorization", "X-Correlation-Id", "Idempotency-Key")
            .WithExposedHeaders("X-Correlation-Id")));

        return services;
    }
}
