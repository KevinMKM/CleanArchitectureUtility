using CleanArchitectureUtility.Core.Abstractions.Translations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Translations.Parrot;

public static class ParrotTranslatorServiceCollectionX
{
    public static IServiceCollection AddParrotTranslator(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ITranslator, ParrotTranslator>();
        services.Configure<ParrotTranslatorOptions>(configuration);
        return services;
    }

    public static IServiceCollection AddParrotTranslator(this IServiceCollection services, IConfiguration configuration, string sectionName)
    {
        services.AddParrotTranslator(configuration.GetSection(sectionName));
        return services;
    }

    public static IServiceCollection AddParrotTranslator(this IServiceCollection services, Action<ParrotTranslatorOptions> setupAction)
    {
        services.AddSingleton<ITranslator, ParrotTranslator>();
        services.Configure(setupAction);
        return services;
    }
}