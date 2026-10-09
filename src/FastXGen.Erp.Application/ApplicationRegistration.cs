using FastXGen.Erp.Application.Features.Notifications;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace FastXGen.Erp.Application;

public static class ApplicationRegistration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(ApplicationRegistration).Assembly, includeInternalTypes: true);

        services.AddScoped<INotificationPublisher, NotificationPublisher>();

        return services;
    }
}
