using FluentValidation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace FastXGen.Erp.IntegrationTests.Api.Probe;

/// <summary>The real API plus the test-only probe controller.</summary>
public class ProbeApiFactory : WebApplicationFactory<Program>
{
    protected virtual int PermitsPerMinute => 1000;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("RateLimiting:PermitsPerMinute", PermitsPerMinute.ToString(System.Globalization.CultureInfo.InvariantCulture));
        builder.ConfigureServices(services =>
        {
            services.AddControllers().AddApplicationPart(typeof(ProbeController).Assembly);
            services.AddScoped<IValidator<ProbeRequest>, ProbeRequestValidator>();
        });
    }
}

/// <summary>Same API with a tiny rate limit, to prove the limiter works.</summary>
public sealed class RateLimitedApiFactory : ProbeApiFactory
{
    public const int Limit = 2;

    protected override int PermitsPerMinute => Limit;
}
