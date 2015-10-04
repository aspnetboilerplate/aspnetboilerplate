using System;
using System.Runtime.Caching;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Abp.Runtime.Caching.Memory
{
    /// <summary>
    /// This class is used to store items to a thread safe and generic cache in a simple manner.
    /// It uses async pattern.
    /// </summary>
    /// <typeparam name="TValue">Value type</typeparam>
    [Obsolete("Inject and use ICacheManager caching.")]
    public class AsyncThreadSafeObjectCache<TValue> where TValue : class
    {
        /// <summary>
        /// The real cache object to store items.
        /// </summary>
        private readonly ObjectCache _cache;

        private readonly AsyncLock _asyncLock = new AsyncLock();

        /// <summary>
        /// <see cref="_defaultCacheItemPolicy"/> is used if no policy is specified.
        /// </summary>
        private readonly CacheItemPolicy _defaultCacheItemPolicy;

        private const int DefaultSlidingCacheDurationAsMinutes = 120;

        /// <summary>
        /// Creates a new <see cref="ThreadSafeObjectCache{TValue}"/> object.
        /// </summary>
        /// <param name="cache">The real cache object to store items</param>
        public AsyncThreadSafeObjectCache(ObjectCache cache)
            : this(cache, new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(DefaultSlidingCacheDurationAsMinutes) })
        {

        }

        /// <summary>
        /// Creates a new <see cref="ThreadSafeObjectCache{TValue}"/> object.
        /// </summary>
        /// <param name="cache">The real cache object</param>
        /// <param name="slidingExpiration">Default cache policy as sliding expiration</param>
        public AsyncThreadSafeObjectCache(ObjectCache cache, TimeSpan slidingExpiration)
            : this(cache, new CacheItemPolicy { SlidingExpiration = slidingExpiration })
        {

        }

        /// <summary>
        /// Creates a new <see cref="ThreadSafeObjectCache{TValue}"/> object.
        /// </summary>
        /// <param name="cache">The real cache object</param>
        /// <param name="absoluteExpiration">Default cache policy as absolute expiration</param>
        public AsyncThreadSafeObjectCache(ObjectCache cache, DateTimeOffset absoluteExpiration)
            : this(cache, new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration })
        {

        }

        /// <summary>
        /// Creates a new <see cref="ThreadSafeObjectCache{TValue}"/> object.
        /// </summary>
        /// <param name="cache">The real cache object</param>
        /// <param name="defaultCacheItemPolicy">Default cache policy</param>
        public AsyncThreadSafeObjectCache(ObjectCache cache, CacheItemPolicy defaultCacheItemPolicy)
        {
            _cache = cache;
            _defaultCacheItemPolicy = defaultCacheItemPolicy;
        }

        /// <summary>
        /// Gets an item from cache if exists, or null.
        /// </summary>
        /// <param name="key">Key to get item</param>
        public TValue Get(string key)
        {
            return _cache.Get(key) as TValue;
        }

        /// <summary>
        /// Gets an item from cache if exists, or calls <paramref name="factoryMethod"/> to create cache item and return it.
        /// </summary>
        /// <param name="key">Key to get item</param>
        /// <param name="factoryMethod">A factory method to create item if it's not exists in cache</param>
        public async Task<TValue> GetAsync(string key, Func<Task<TValue>> factoryMethod)
        {
            return await GetAsync(key, () => _defaultCacheItemPolicy, factoryMethod);
        }

        /// <summary>
        /// Gets an item from cache if exists, or calls <paramref name="factoryMethod"/> to create cache item and return it.
        /// </summary>
        /// <param name="key">Key to get item</param>
        /// <param name="slidingExpiration">Sliding expiration policy</param>
        /// <param name="factoryMethod">A factory method to create item if it's not exists in cache</param>
        public async Task<TValue> GetAsync(string key, TimeSpan slidingExpiration, Func<Task<TValue>> factoryMethod)
        {
            return await GetAsync(key, () => new CacheItemPolicy { SlidingExpiration = slidingExpiration }, factoryMethod);
        }

        /// <summary>
        /// Gets an item from cache if exists, or calls <paramref name="factoryMethod"/> to create cache item and return it.
        /// </summary>
        /// <param name="key">Key to get item</param>
        /// <param name="absoluteExpiration">Absolute expiration policy</param>
        /// <param name="factoryMethod">A factory method to create item if it's not exists in cache</param>
        public async Task<TValue> GetAsync(string key, DateTimeOffset absoluteExpiration, Func<Task<TValue>> factoryMethod)
        {
            return await GetAsync(key, () => new CacheItemPolicy { AbsoluteExpiration = absoluteExpiration }, factoryMethod);
        }

        /// <summary>
        /// Gets an item from cache if exists, or calls <paramref name="factoryMethod"/> to create cache item and return it.
        /// </summary>
        /// <param name="key">Key to get item</param>
        /// <param name="cacheItemPolicy">Cache policy creation method (called only if item is being added to the cache)</param>
        /// <param name="factoryMethod">A factory method to create item if it's not exists in cache</param>
        public async Task<TValue> GetAsync(string key, Func<CacheItemPolicy> cacheItemPolicy, Func<Task<TValue>> factoryMethod)
        {
            var cacheKey = key;
            var item = (TValue)_cache[cacheKey];
            if (item == null)
            {
                using (await _asyncLock.LockAsync())
                {
                    item = (TValue)_cache[cacheKey];
                    if (item == null)
                    {
                        item = await factoryMethod();
                        _cache.Set(cacheKey, item, cacheItemPolicy());
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Adds/replaces an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <param name="value">Value of the item</param>
        public void Set(string key, TValue value)
        {
            _cache.Set(key, value, _defaultCacheItemPolicy);
        }

        /// <summary>
        /// Adds/replaces an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <param name="value">Value of the item</param>
        /// <param name="slidingExpiration">Sliding expiration policy</param>
        public void Set(string key, TValue value, TimeSpan slidingExpiration)
        {
            _cache.Set(key, value, new CacheItemPolicy { SlidingExpiration = slidingExpiration });
        }

        /// <summary>
        /// Adds/replaces an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <param name="value">Value of the item</param>
        /// <param name="absoluteExpiration">Absolute expiration policy</param>
        public void Set(string key, TValue value, DateTimeOffset absoluteExpiration)
        {
            _cache.Set(key, value, absoluteExpiration);
        }

        /// <summary>
        /// Adds/replaces an item in the cache.
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <param name="value">Value of the item</param>
        /// <param name="cacheItemPolicy">Cache item policy</param>
        public void Set(string key, TValue value, CacheItemPolicy cacheItemPolicy)
        {
            _cache.Set(key, value, cacheItemPolicy);
        }

        /// <summary>
        /// Removes an item from the cache (if it exists).
        /// </summary>
        /// <param name="key">Key of the item</param>
        /// <returns>Removed item (if it exists)</returns>
        public TValue Remove(string key)
        {
            return _cache.Remove(key) as TValue;
        }
    }
}
