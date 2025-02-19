using CleanArchitectureUtility.Extensions.Abstractions.Loggers;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Endpoints.WebApi.Extensions.DependencyInjection;

public static class AddServicesExtensions
{
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddScoped<IScopeInformation, ScopeInformation>();
        return services;
    }
}