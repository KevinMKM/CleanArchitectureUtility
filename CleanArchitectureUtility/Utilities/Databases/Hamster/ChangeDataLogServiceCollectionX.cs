using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Databases.Hamster;

public static class ChangeDataLogServiceCollectionX
{
    public static IServiceCollection AddHamsterChangeDataLog(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ChangeDataLogHamsterOptions>(configuration);
        return services;
    }

    public static IServiceCollection AddHamsterChangeDataLog(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        services.AddHamsterChangeDataLog(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddHamsterChangeDataLog(this IServiceCollection services, Action<ChangeDataLogHamsterOptions> setupAction)
    {
        services.Configure(setupAction);
        return services;
    }
}