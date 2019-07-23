using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheMultipleKeysOperations<TKey, TSingleValue> : ICacheMultipleKeysOperations<TKey, TSingleValue, TSingleValue[]>
    {
    }

    public interface ICacheMultipleKeysOperations<TKey, TSingleValue, TMultipleValues>
    {
        /// <summary>
        /// Gets items from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached item</returns>
        TMultipleValues Get(TKey[] keys, Func<TKey, TSingleValue> factory);

        /// <summary>
        /// Gets items from the cache.
        /// This method hides cache provider failures (and logs them),
        /// uses the factory method to get the object if cache provider fails.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <param name="factory">Factory method to create cache item if not exists</param>
        /// <returns>Cached items</returns>
        Task<TMultipleValues> GetAsync(TKey[] keys, Func<TKey, Task<TSingleValue>> factory);

        /// <summary>
        /// Gets items from the cache. For every key that is not found, a null value is returned.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Cached items</returns>
        TMultipleValues GetOrDefault(TKey[] keys);

        /// <summary>
        /// Gets items from the cache. For every key that is not found, a null value is returned.
        /// </summary>
        /// <param name="keys">Keys</param>
        /// <returns>Cached items</returns>
        Task<TMultipleValues> GetOrDefaultAsync(TKey[] keys);

        /// <summary>
        /// Saves/Overrides items in the cache by the pairs.
        /// Use one of the expire times at most (<paramref name="slidingExpireTime"/> or <paramref name="absoluteExpireTime"/>).
        /// If none of them is specified, then
        /// <see cref="ICache.DefaultAbsoluteExpireTime"/> will be used if it's not null. Othewise, <see cref="DefaultSlidingExpireTime"/>
        /// will be used.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        void Set(KeyValuePair<TKey, TSingleValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Saves/Overrides items in the cache by the pairs.
        /// Use one of the expire times at most (<paramref name="slidingExpireTime"/> or <paramref name="absoluteExpireTime"/>).
        /// If none of them is specified, then
        /// <see cref="ICache.DefaultAbsoluteExpireTime"/> will be used if it's not null. Othewise, <see cref="DefaultSlidingExpireTime"/>
        /// will be used.
        /// </summary>
        /// <param name="pairs">Pairs</param>
        /// <param name="slidingExpireTime">Sliding expire time</param>
        /// <param name="absoluteExpireTime">Absolute expire time</param>
        Task SetAsync(KeyValuePair<TKey, TSingleValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        /// <summary>
        /// Removes cache items by their keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        void Remove(TKey[] keys);

        /// <summary>
        /// Removes cache items by their keys.
        /// </summary>
        /// <param name="keys">Keys</param>
        Task RemoveAsync(TKey[] keys);
    }
}
