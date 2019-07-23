using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheSingleKeyOperations<TKey, TValue>
    {
        /// <summary>
        /// Gets an item from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        TValue Get(TKey key, Func<TKey, TValue> factory);

        /// <summary>
        /// Gets an item from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory);

        /// <summary>
        /// Gets an item from the cache or null if not found.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item or null if not found</returns>
        TValue GetOrDefault(TKey key);

        /// <summary>
        /// Gets an item from the cache or null if not found.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item or null if not found</returns>
        Task<TValue> GetOrDefaultAsync(TKey key);

        /// <summary>
        /// Saves/Overrides an item in the cache by a key.
        /// Use one of the expire times at most (<paramref name="slidingExpireTime"/> or <paramref name="absoluteExpireTime"/>).
        /// If none of them is specified, then
        /// <see cref="ICacheOptions.DefaultAbsoluteExpireTime"/> will be used if it's not null. Othewise, <see cref="ICacheOptions.DefaultSlidingExpireTime"/>
        /// will be used.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Saves/Overrides an item in the cache by a key.
        /// Use one of the expire times at most (<paramref name="slidingExpireTime"/> or <paramref name="absoluteExpireTime"/>).
        /// If none of them is specified, then
        /// <see cref="ICacheOptions.DefaultAbsoluteExpireTime"/> will be used if it's not null. Othewise, <see cref="ICacheOptions.DefaultSlidingExpireTime"/>
        /// will be used.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(TKey key);

        /// <summary>
        /// Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        Task RemoveAsync(TKey key);
    }
}
