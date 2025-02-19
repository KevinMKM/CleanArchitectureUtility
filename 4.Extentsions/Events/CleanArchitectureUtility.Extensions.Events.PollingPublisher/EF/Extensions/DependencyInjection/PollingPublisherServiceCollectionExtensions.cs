using CleanArchitectureUtility.Extensions.Events.PollingPublisher.EF.Options;
using CleanArchitectureUtility.Extensions.Events.PollingPublisher.EF.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Extensions.Events.PollingPublisher.EF.Extensions.DependencyInjection;

public static class PollingPublisherServiceCollectionExtensions
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

    public static IServiceCollection AddPollingPublisher(this IServiceCollection services,
        Action<PollingPublisherOptions> setupAction)
    {
        services.Configure(setupAction);
        AddServices(services);
        return services;
    }

    private static void AddServices(IServiceCollection services)
    {
        services.AddHostedService<PoolingPublisherBackgroundService>();
    }
}