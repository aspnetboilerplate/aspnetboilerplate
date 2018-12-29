using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// An interface to work with cache in a typed manner.
    /// Use <see cref="CacheExtensions.AsTyped{TKey,TValue}"/> method
    /// to convert a <see cref="ICache"/> to this interface.
    /// </summary>
    /// <typeparam name="TKey">Key type for cache items</typeparam>
    /// <typeparam name="TValue">Value type for cache items</typeparam>
    public interface ITypedCache<TKey, TValue> : IDisposable
    {
        /// <summary>
        /// Unique name of the cache.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Default sliding expire time of cache items.
        /// </summary>
        TimeSpan DefaultSlidingExpireTime { get; set; }

        /// <summary>
        /// Gets the internal cache.
        /// </summary>
        ICache InternalCache { get; }

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        TValue Get(TKey key, Func<TKey, TValue> factory);

        /// <summary>
        /// Gets items from the cache.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached items</returns>
        TValue[] Get(TKey[] keys, Func<TKey, TValue> factory);

        /// <summary>
        /// Gets an item from the cache.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory);

        /// <summary>
        /// Gets items from the cache.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory);

        /// <summary>
        /// Gets an item from the cache or null if not found.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item or null if not found</returns>
        TValue GetOrDefault(TKey key);

        /// <summary>
        /// Gets items from the cache. For every key that is not found, a null value is returned.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Cached items</returns>
        TValue[] GetOrDefault(TKey[] keys);

        /// <summary>
        /// Gets an item from the cache or null if not found.
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Cached item or null if not found</returns>
        Task<TValue> GetOrDefaultAsync(TKey key);

        /// <summary>
        /// Gets items from the cache. For every key that is not found, a null value is returned.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Cached items</returns>
        Task<TValue[]> GetOrDefaultAsync(TKey[] keys);

        /// <summary>
        /// Saves/Overrides an item in the cache by a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Saves/Overrides items in the cache by the pairs.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Saves/Overrides an item in the cache by a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="value">Value</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Saves/Overrides items in the cache by the pairs.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        void Remove(TKey key);

        /// <summary>
        /// Removes cache items by their keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        void Remove(TKey[] keys);

        /// <summary>
        /// Removes a cache item by it's key (does nothing if given key does not exists in the cache).
        /// </summary>
        /// <param name="key">Key</param>
        Task RemoveAsync(TKey key);

        /// <summary>
        /// Removes cache items by their keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        Task RemoveAsync(TKey[] keys);

        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        Task ClearAsync();
    }
}