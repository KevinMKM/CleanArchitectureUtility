using System.Security.Claims;
using CleanArchitectureUtility.Core.Abstractions.Caching;
using CleanArchitectureUtility.Utilities.Authentication.Models;
using CleanArchitectureUtility.Utilities.Authentication.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using IdentityModel.Client;

namespace CleanArchitectureUtility.Utilities.Authentication.Extensions;

public static class ProviderX
{
    public static string AddProviderHttpClient(this ProviderOption provider, IServiceCollection services)
    {
        var httpClientName = string.IsNullOrWhiteSpace(provider.HttpClientFactoryName)
            ? $"Provider{provider.Priority}HttpClient"
            : provider.HttpClientFactoryName;

        IHttpClientBuilder httpClientBuilder =
            services.AddHttpClient(httpClientName, option => { option.BaseAddress = new Uri(provider.Authority); });
        if (provider.IgnoreSSL)
        {
            httpClientBuilder.ConfigurePrimaryHttpMessageHandler(
                () => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = delegate { return true; }
                });
        }

        return httpClientName;
    }

    public static async Task<List<Claim>> GetUserInfoClaims(this ProviderOption provider, HttpContext httpContext, string httpClientName)
    {
        var token = httpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "")
                    ?? throw new ArgumentNullException($"{provider.Scheme} ({provider.Authority}) , token is null");

        var client = httpContext.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient(httpClientName);

        var userInfoClaims = await provider.UserInfoEndpointCaller(httpContext, client, token);

        return userInfoClaims;
    }

    public static async Task<List<Claim>> UserInfoEndpointCaller(this ProviderOption provider, HttpContext httpContext, HttpClient client, string token)
    {
        List<Claim> claims;
        if (provider.RegisterUserInfoClaims.CachingData)
        {
            var cacheAdapter = httpContext.RequestServices.GetRequiredService<ICacheAdapter>()
                               ?? throw new ArgumentNullException($"{provider.Scheme} ({provider.Authority}) , cache adapter is null");

            var cacheKey = GenerateCacheKey(provider, token);

            claims = cacheAdapter.Get<List<ClaimCacheModel>>(cacheKey)?.Select(cacheModel => cacheModel.ToClaim()).ToList() ?? new List<Claim>();
            if (claims.Count != 0)
                return claims;
            claims = await provider.CallUserInfoEndpoint(client, token);
            switch (provider.RegisterUserInfoClaims.CacheExpirationType)
            {
                case CacheExpirationType.Absolute:
                    cacheAdapter.Add(cacheKey,
                        claims.Select(ClaimCacheModel.FromClaim),
                        DateTime.Now.AddSeconds(provider.RegisterUserInfoClaims.CacheExpirationInSeconds),
                        null);
                    break;
                case CacheExpirationType.Sliding:
                    cacheAdapter.Add(cacheKey,
                        claims.Select(ClaimCacheModel.FromClaim),
                        null,
                        TimeSpan.FromSeconds(provider.RegisterUserInfoClaims.CacheExpirationInSeconds));
                    break;
                default:
                    throw new InvalidOperationException($"Invalid cache expiration type for {provider.Scheme} ({provider.Authority})");
            }
        }
        else
            claims = await provider.CallUserInfoEndpoint(client, token);

        return claims;
    }

    private static string GenerateCacheKey(this ProviderOption provider, string token)
    {
        var cacheKeyText = $"{provider.RegisterUserInfoClaims.CacheKeyPrefix}{provider.Scheme}_{token}";
        var cacheKey = provider.RegisterUserInfoClaims.CacheKeyFormat == CacheKeyFormat.Base64 ? cacheKeyText.ToBase64Encode() : cacheKeyText;
        return cacheKey;
    }

    public static async Task<List<Claim>> CallUserInfoEndpoint(this ProviderOption provider, HttpClient client, string token)
    {
        var response = await client.GetUserInfoAsync(new UserInfoRequest
        {
            Address = provider.EndpointsPath?.UserInfoEndpoint ?? (await client.GetDiscoveryDocumentAsync())?.UserInfoEndpoint?.Replace(provider.Authority, ""),
            Token = token
        });

        if (response.IsError)
            throw new Exception(response.Error);

        return response.Claims.ToList();
    }

    private static string ToBase64Encode(this string plainText) => Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(plainText));
}