using System.Text;
using CleanArchitectureUtility.Core.Abstractions.Caching;
using CleanArchitectureUtility.Core.Abstractions.Serializers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureUtility.Utilities.Caching.Redis;

public class DistributedRedisCacheAdapter : ICacheAdapter
{
    private readonly IDistributedCache _cache;
    private readonly IJsonSerializer _jsonSerializer;
    private readonly ILogger<DistributedRedisCacheAdapter> _logger;

    public DistributedRedisCacheAdapter(IDistributedCache cache,
        IJsonSerializer jsonSerializer,
        ILogger<DistributedRedisCacheAdapter> logger)
    {
        _cache = cache;
        _jsonSerializer = jsonSerializer;
        _logger = logger;
        _logger.LogInformation("DistributedCache Redis Adapter Start working");
    }

    public void Add<TInput>(string key, TInput obj, DateTime? absoluteExpiration, TimeSpan? slidingExpiration)
    {
        _logger.LogTrace($"InMemoryCache Adapter Cache {typeof(TInput)} with key : {key} " +
                         $", with data : {_jsonSerializer.Serialize(obj)} " +
                         $", with absoluteExpiration : {absoluteExpiration} " +
                         $", with slidingExpiration : {slidingExpiration}");

        var option = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = absoluteExpiration,
            SlidingExpiration = slidingExpiration
        };

        _cache.Set(key, Encoding.UTF8.GetBytes(_jsonSerializer.Serialize(obj)), option);
    }

    public TOutput? Get<TOutput>(string key)
    {
        var result = _cache.GetString(key);
        if (!string.IsNullOrWhiteSpace(result))
        {
            _logger.LogTrace($"DistributedCache Redis Adapter Successful Get Cache with key : {key} and data : {_jsonSerializer.Serialize(result)}");
            return _jsonSerializer.Deserialize<TOutput>(result);
        }

        _logger.LogTrace($"DistributedCache Redis Adapter Failed Get Cache with key : {key}");

        return default;
    }

    public void RemoveCache(string key)
    {
        _logger.LogTrace("DistributedCache Redis Adapter Remove Cache with key : {key}", key);
        _cache.Remove(key);
    }
}