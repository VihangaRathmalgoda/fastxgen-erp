using System.Text.Json.Serialization;
using FastXGen.Erp.Api.Errors;
using FastXGen.Erp.Api.Security;
using FastXGen.Erp.Api.Tenancy;
using FastXGen.Erp.Api.Validation;
using FastXGen.Erp.Application.Abstractions.Tenancy;

namespace FastXGen.Erp.Api.Setup;

internal static class ApiServicesSetup
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers(options => options.Filters.Add<ValidationFilter>())
            .AddJsonOptions(options =>
            {
                var json = options.JsonSerializerOptions;

                // Unknown properties are rejected, so a client cannot slip extra fields into a request.
                json.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
                json.MaxDepth = 32;
                json.Converters.Add(new JsonStringEnumConverter(namingPolicy: null, allowIntegerValues: false));

                // Parser exception text can reveal internals; clients get a generic field message instead.
                options.AllowInputFormatterExceptionMessages = false;
            })
            .ConfigureApiBehaviorOptions(options =>
                options.InvalidModelStateResponseFactory = context =>
                    ProblemResponses.ValidationResult(context.HttpContext, context.ModelState));

        services.AddProblemDetails(options => options.CustomizeProblemDetails = ProblemResponses.Customize);
        services.AddExceptionHandler<AppExceptionHandler>();

        services.AddOpenApi();
        services.AddHttpContextAccessor();
        services.AddAuthorization();
        services.AddScoped<ITenantContext, ClaimsTenantContext>();

        services.AddApiRateLimiting(configuration);
        services.AddApiCors(configuration);

        return services;
    }
}
