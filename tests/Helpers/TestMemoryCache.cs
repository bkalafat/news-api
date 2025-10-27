using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace NewsApi.Tests.Helpers;

public class TestMemoryCache : IMemoryCache
{
    private readonly Dictionary<object, object?> _cache = new();

    public ICacheEntry CreateEntry(object key)
    {
        return new TestCacheEntry(key, this);
    }

    public void Dispose()
    {
        _cache.Clear();
    }

    public void Remove(object key)
    {
        _cache.Remove(key);
    }

    public bool TryGetValue(object key, out object? value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set(object key, object? value)
    {
        _cache[key] = value;
    }
}

public class TestCacheEntry : ICacheEntry
{
    private readonly TestMemoryCache _cache;

    public TestCacheEntry(object key, TestMemoryCache cache)
    {
        Key = key;
        _cache = cache;
    }

    public object Key { get; }
    public object? Value { get; set; }
    public DateTimeOffset? AbsoluteExpiration { get; set; }
    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
    public TimeSpan? SlidingExpiration { get; set; }
    public IList<IChangeToken> ExpirationTokens { get; } = new List<IChangeToken>();
    public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } =
        new List<PostEvictionCallbackRegistration>();
    public CacheItemPriority Priority { get; set; } = CacheItemPriority.Normal;
    public long? Size { get; set; }

    public void Dispose()
    {
        _cache.Set(Key, Value);
    }
}
