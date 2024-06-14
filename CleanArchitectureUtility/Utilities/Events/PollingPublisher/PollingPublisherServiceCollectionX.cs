using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Events.PollingPublisher;

public static class PollingPublisherServiceCollectionX
{
    public static IServiceCollection AddPollingPublisher(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PollingPublisherOptions>(configuration);
        AddServices(services);
        return services;
    }

    public static IServiceCollection AddPollingPublisher(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        services.AddPollingPublisher(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddPollingPublisher(this IServiceCollection services, Action<PollingPublisherOptions> setupAction)
    {
        services.Configure(setupAction);
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services) => services.AddHostedService<PoolingPublisherBackgroundService>();
}