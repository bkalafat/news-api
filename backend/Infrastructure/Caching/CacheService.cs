using System;
using Microsoft.Extensions.Caching.Memory;

namespace NewsApi.Infrastructure.Caching;

public interface ICacheService
{
    T? Get<T>(string key);
    void Set<T>(string key, T value, TimeSpan expiration);
    void Remove(string key);
    bool TryGetValue<T>(string key, out T? value);
}

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
    }

    public T? Get<T>(string key)
    {
        return _memoryCache.Get<T>(key);
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _memoryCache.Set(key, value, expiration);
    }

    public void Remove(string key)
    {
        _memoryCache.Remove(key);
    }

    public bool TryGetValue<T>(string key, out T? value)
    {
        return _memoryCache.TryGetValue(key, out value);
    }
}
