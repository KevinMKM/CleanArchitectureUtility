using CleanArchitectureUtility.Core.Abstractions.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitectureUtility.Utilities.Caching.InMemory;

public static class InMemoryCachingServiceCollectionX
{
    public static IServiceCollection AddInMemoryCaching(this IServiceCollection services)
        => services.AddMemoryCache().AddTransient<ICacheAdapter, InMemoryCacheAdapter>();
}