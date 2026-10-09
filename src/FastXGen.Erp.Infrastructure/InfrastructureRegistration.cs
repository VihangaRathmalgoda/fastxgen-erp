using FastXGen.Erp.Application.Abstractions.Time;
using FastXGen.Erp.Infrastructure.Time;
using Microsoft.Extensions.DependencyInjection;

namespace FastXGen.Erp.Infrastructure;

public static class InfrastructureRegistration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IClock, SystemClock>();
        return services;
    }
}
