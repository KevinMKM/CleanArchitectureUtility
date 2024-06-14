using CleanArchitectureUtility.Core.Abstractions.UsersManagement;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.UsersManagement;

public static class UserInfoServiceCollectionX
{
    public static IServiceCollection AddWebUserInfoService(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<UserManagementOptions>(configuration);
        services.AddSingleton<IUserInfoService, WebUserInfoService>();
        return services;
    }


    public static IServiceCollection AddWebUserInfoService(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        services.AddWebUserInfoService(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddWebUserInfoService(this IServiceCollection services, Action<UserManagementOptions> setupAction)
    {
        services.Configure(setupAction);
        services.AddSingleton<IUserInfoService, WebUserInfoService>();
        return services;
    }
}