using FastXGen.Erp.Application.Abstractions.Notifications;
using FastXGen.Erp.Persistence.Health;
using FastXGen.Erp.Persistence.Repositories.Notifications;
using FastXGen.Erp.Persistence.Sql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FastXGen.Erp.Persistence;

public static class PersistenceRegistration
{
    /// <summary>Health checks with this tag need the database; liveness checks do not.</summary>
    public const string ReadinessTag = "ready";

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName));

        services.AddScoped<ISqlConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<ISqlExecutor, SqlExecutor>();

        services.AddScoped<INotificationStore, NotificationStore>();

        services.AddHealthChecks()
            .AddCheck<DatabaseHealthCheck>("database", tags: [ReadinessTag]);

        return services;
    }
}
