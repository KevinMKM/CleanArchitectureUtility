using CleanArchitectureUtility.Extensions.Abstractions.Caching;
using CleanArchitectureUtility.Extensions.Abstractions.Serializers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Extensions.Caching.InMemory.Services;

public class InMemoryCacheAdapter : ICacheAdapter
{
    private readonly IMemoryCache _memoryCache;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<InMemoryCacheAdapter> _logger;

    public InMemoryCacheAdapter(IMemoryCache memoryCache, IJsonSerializer jsonSerializer, ILogger<InMemoryCacheAdapter> logger)
    {
        _memoryCache = memoryCache;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
        _logger.LogInformation("InMemoryCache Adapter Start working");
    }

    public void Add<TInput>(string key, TInput obj, DateTime? absoluteExpiration, TimeSpan? slidingExpiration)
    {
        _logger.LogTrace($"InMemoryCache Adapter Cache {typeof(TInput)} with key : {key} " +
                         $", with data : {_jsonSerializer.Serialize(obj)} " +
                         $", with absoluteExpiration : {absoluteExpiration.ToString()} " +
                         $", with slidingExpiration : {slidingExpiration.ToString()}");

        _memoryCache.Set(key, obj, new MemoryCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration,
        });
    }

    public TOutput? Get<TOutput>(string key)
    {
        _logger.LogTrace($"InMemoryCache Adapter Try Get Cache with key : {key}");
        var result = _memoryCache.TryGetValue(key, out TOutput? resultObject);
        _logger.LogTrace(result
            ? $"InMemoryCache Adapter Successful Get Cache with key : {key} and data : {_jsonSerializer.Serialize(resultObject)}"
            : $"InMemoryCache Adapter Failed Get Cache with key : {key}");

        return resultObject;
    }

    public void RemoveCache(string key)
    {
        _logger.LogTrace($"InMemoryCache Adapter Remove Cache with key : {key}");
        _memoryCache.Remove(key);
    }
}