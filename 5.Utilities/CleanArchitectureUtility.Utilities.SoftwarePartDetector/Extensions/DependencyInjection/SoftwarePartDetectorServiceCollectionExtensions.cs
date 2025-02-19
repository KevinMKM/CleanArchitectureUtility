using CleanArchitectureUtility.Utilities.SoftwarePartDetector.Authentications;
using CleanArchitectureUtility.Utilities.SoftwarePartDetector.Detectors;
using CleanArchitectureUtility.Utilities.SoftwarePartDetector.Options;
using CleanArchitectureUtility.Utilities.SoftwarePartDetector.Publishers;
using CleanArchitectureUtility.Utilities.SoftwarePartDetector.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.SoftwarePartDetector.Extensions.DependencyInjection;

public static class SoftwarePartDetectorServiceCollectionExtensions
{
    public static IServiceCollection AddSoftwarePartDetector(this IServiceCollection services, IConfiguration configuration, string sectionName)
        => services.AddSoftwarePartDetector(configuration.GetSection(sectionName));

    public static IServiceCollection AddSoftwarePartDetector(this IServiceCollection services, IConfiguration configuration)
        => services.AddServices(configuration.Get<SoftwarePartDetectorOptions>()).Configure<SoftwarePartDetectorOptions>(configuration);

    public static IServiceCollection AddSoftwarePartDetector(this IServiceCollection services, Action<SoftwarePartDetectorOptions> setupAction)
    {
        var option = new SoftwarePartDetectorOptions();
        setupAction.Invoke(option);
        return services.AddServices(option).Configure(setupAction);
    }

    private static IServiceCollection AddServices(this IServiceCollection services, SoftwarePartDetectorOptions option)
    {
        services.AddTransient<ControllersAndActionDetector>();
        services.AddTransient<Detectors.SoftwarePartDetector>();
        services.AddTransient<ISoftwarePartPublisher, SoftwarePartWebPublisher>();
        services.AddTransient<ISoftwarePartAuthentication, SoftwarePartAuthentication>();
        services.AddTransient<SoftwarePartDetectorService>();
        var publisherHttpClient = services.AddHttpClient<ISoftwarePartPublisher, SoftwarePartWebPublisher>();
        var oidcHttpClient = services.AddHttpClient<ISoftwarePartAuthentication, SoftwarePartAuthentication>();

        if (!option.FakeSSL)
            return services;

        publisherHttpClient.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true });
        oidcHttpClient.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            { ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true });

        return services;
    }
}