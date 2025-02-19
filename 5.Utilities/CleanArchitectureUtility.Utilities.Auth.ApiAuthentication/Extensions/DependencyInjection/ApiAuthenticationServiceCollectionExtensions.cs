using CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Extensions.DependencyInjection;

public static class ApiAuthenticationServiceCollectionExtensions
{
    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration, string sectionName)
        => services.AddApiAuthentication(configuration.GetSection(sectionName));

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ApiAuthenticationOption>(configuration);
        var option = configuration.Get<ApiAuthenticationOption>() ?? new ApiAuthenticationOption();
        return services.AddAuthentication(option);
    }

    public static IServiceCollection AddApiAuthentication(this IServiceCollection services, Action<ApiAuthenticationOption> action)
    {
        services.Configure(action);
        var option = new ApiAuthenticationOption();
        action.Invoke(option);
        return services.AddAuthentication(option);
    }

    private static IServiceCollection AddAuthentication(this IServiceCollection services, ApiAuthenticationOption option)
    {
        if (!option.Active) 
            return services;
        var defaultProvider = option.DefaultProvider ?? throw new InvalidOperationException($"DefaultProvider is null");
        var authenticationBuilder = services.AddAuthentication(defaultProvider.Scheme);
        foreach (var provider in option.EnabledProviders)
            switch (provider.TokenTypeSupport)
            {
                case TokenType.Jwt:
                    authenticationBuilder.AddJwtTokenSupport(services, provider);
                    break;

                case TokenType.Reference:
                    authenticationBuilder.AddReferenceTokenSupport(services, provider);
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Invalid token type for {provider.Scheme} ({provider.Authority})");
            }

        services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .AddAuthenticationSchemes(option.EnabledProviders.Select(c => c.Scheme).ToArray())
            .Build());

        return services;
    }
}