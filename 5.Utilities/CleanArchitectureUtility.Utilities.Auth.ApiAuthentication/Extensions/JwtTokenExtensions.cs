using System.Security.Claims;
using CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace CleanArchitectureUtility.Utilities.Auth.ApiAuthentication.Extensions;

public static class JwtTokenExtensions
{
    public static AuthenticationBuilder AddJwtTokenSupport(this AuthenticationBuilder authenticationBuilder, IServiceCollection services, ProviderOption provider)
    {
        var httpClientName = provider.AddProviderHttpClient(services);
        authenticationBuilder.AddJwtBearer(provider.Scheme, option =>
        {
            option.Authority = provider.Authority;
            option.Audience = provider.JwtTokenConfig.Audience;
            option.RequireHttpsMetadata = provider.JwtTokenConfig.RequireHttpsMetadata;
            if (provider.IgnoreSSL)
                option.BackchannelHttpHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = delegate { return true; }
                };

            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = provider.JwtTokenConfig.ValidateAudience,
                ValidateIssuer = provider.JwtTokenConfig.ValidateIssuer,
                ValidateIssuerSigningKey = provider.JwtTokenConfig.ValidateIssuerSigningKey,
            };

            if (provider.EndpointsPath is not null)
                option.Configuration = new OpenIdConnectConfiguration
                {
                    AuthorizationEndpoint = provider.EndpointsPath.AuthorizeEndpoint,
                    TokenEndpoint = provider.EndpointsPath.TokenEndpoint,
                    UserInfoEndpoint = provider.EndpointsPath.UserInfoEndpoint,
                    IntrospectionEndpoint = provider.EndpointsPath.IntrospectionEndpoint
                };

            option.Events = new JwtBearerEvents
            {
                OnTokenValidated = async (context) =>
                {
                    if (context.Principal is null)
                        throw new ArgumentNullException(
                            $"{provider.Scheme} ({provider.Authority}) , principal is null");

                    if (provider.RegisterUserInfoClaims.Enabled && context.Principal.HasSubClaim(provider.UserIdentifierClaimType))
                    {
                        List<Claim> claims = await provider.GetUserInfoClaims(context.HttpContext, httpClientName);
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