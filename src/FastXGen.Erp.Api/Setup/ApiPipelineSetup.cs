using FastXGen.Erp.Api.Middleware;
using FastXGen.Erp.Api.Security;
using FastXGen.Erp.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace FastXGen.Erp.Api.Setup;

internal static class ApiPipelineSetup
{
    /// <summary>The request pipeline. Order matters; read top to bottom as the path of a request.</summary>
    public static WebApplication UseApi(this WebApplication app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<SecurityHeadersMiddleware>();
        app.UseExceptionHandler();
        app.UseStatusCodePages();

        if (!app.Environment.IsDevelopment())
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseCors(CorsSetup.PolicyName);

        // Authentication (Phase 1) goes here, before rate limiting, so limits apply per user.
        app.UseRateLimiter();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.MapControllers();

        // Liveness: the process is up. Readiness: dependencies (database) are reachable.
        app.MapHealthChecks(ApiRoutes.HealthLive, new HealthCheckOptions { Predicate = _ => false }).AllowAnonymous();
        app.MapHealthChecks(ApiRoutes.HealthReady, new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(PersistenceRegistration.ReadinessTag),
        }).AllowAnonymous();

        return app;
    }
}
