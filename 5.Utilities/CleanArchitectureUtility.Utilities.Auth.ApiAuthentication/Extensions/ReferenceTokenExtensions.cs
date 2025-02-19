using System.Security.Claims;
using CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Extensions;

public static class ReferenceTokenExtensions
{
    public static AuthenticationBuilder AddReferenceTokenSupport(this AuthenticationBuilder authenticationBuilder, IServiceCollection services, ProviderOption provider)
    {
        if (string.IsNullOrWhiteSpace(provider.ReferenceTokenConfig.ClientId) || string.IsNullOrWhiteSpace(provider.ReferenceTokenConfig.ClientSecret))
            throw new ArgumentNullException($"{provider.Scheme} ({provider.Authority}) , ClientId or ClientSecret is null or white space or empty");

        var httpClientName = provider.AddProviderHttpClient(services);
        authenticationBuilder.AddOAuth2Introspection(provider.Scheme, option =>
        {
            option.Authority = provider.Authority;
            if (!string.IsNullOrEmpty(provider.EndpointsPath?.IntrospectionEndpoint))
                option.IntrospectionEndpoint = $"{provider.Authority}{provider.EndpointsPath.IntrospectionEndpoint}";

            option.ClientId = provider.ReferenceTokenConfig.ClientId;
            option.ClientSecret = provider.ReferenceTokenConfig.ClientSecret;
            option.Events = new OAuth2IntrospectionEvents
            {
                OnTokenValidated = async (context) =>
                {
                    if (context.Principal is null)
                        throw new ArgumentNullException($"{provider.Scheme} ({provider.Authority}) , principal is null");

                    if (provider.RegisterUserInfoClaims.Enabled && context.Principal.HasSubClaim(provider.UserIdentifierClaimType))
                    {
                        var claims = await provider.GetUserInfoClaims(context.HttpContext, httpClientName);
                        context.Principal.AddIdentity(context.Principal.CreateClaimsIdentity(claims));
                    }

                    if (provider.UserClaimRules.Count != 0 || provider.UserClaimAddOns.Count != 0)
                        context.Principal = context.Principal.ClonePrincipalWithConvertedClaims(provider);
                }
            };
        });

        return authenticationBuilder;
    }
}