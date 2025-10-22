using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace NewsApi.Tests.Helpers;

/// <summary>
/// A no-op implementation of IMemoryCache that doesn't actually cache anything.
/// Used for integration tests to prevent cross-test data contamination.
/// </summary>
public class NullMemoryCache : IMemoryCache
{
    public ICacheEntry CreateEntry(object key)
    {
        return new NullCacheEntry(key);
    }

    public void Remove(object key)
    {
        // No-op
    }

    public bool TryGetValue(object key, out object? value)
    {
        value = null;
        return false;
    }

    public void Dispose()
    {
        // No-op
    }

    private class NullCacheEntry : ICacheEntry
    {
        public NullCacheEntry(object key)
        {
            Key = key;
        }

        public object Key { get; }
        public object? Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
        public IList<IChangeToken> ExpirationTokens { get; } = new List<IChangeToken>();
        public IList<PostEvictionCallbackRegistration> PostEvictionCallbacks { get; } = new List<PostEvictionCallbackRegistration>();
        public CacheItemPriority Priority { get; set; }
        public long? Size { get; set; }

        public void Dispose()
        {
            // No-op
        }
    }
}
