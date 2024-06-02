using CleanArchitectureUtility.Core.Abstractions.Logger;
using CleanArchitectureUtility.Utilities.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Endpoints.API.Extensions.DependencyInjection;

public static class AddServiceExtensions
{
    public static IServiceCollection AddUtilityServices(this IServiceCollection services)
    {
        services.AddScoped<IScopeInformation, ScopeInformation>();
        services.AddTransient<CommonService>();
        return services;
    }
}