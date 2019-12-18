using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Implements <see cref="ITypedCache{TKey,TValue}"/> to wrap a <see cref="ICache"/>.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class TypedCacheWrapper<TKey, TValue> : ITypedCache<TKey, TValue>
    {
        public string Name
        {
            get { return InternalCache.Name; }
        }

        public TimeSpan DefaultSlidingExpireTime
        {
            get { return InternalCache.DefaultSlidingExpireTime; }
            set { InternalCache.DefaultSlidingExpireTime = value; }
        }
        public TimeSpan? DefaultAbsoluteExpireTime
        {
            get { return InternalCache.DefaultAbsoluteExpireTime; }
            set { InternalCache.DefaultAbsoluteExpireTime = value; }
        }

        public ICache InternalCache { get; private set; }

        /// <summary>
        /// Creates a new <see cref="TypedCacheWrapper{TKey,TValue}"/> object.
        /// </summary>
        /// <param name="internalCache">The actual internal cache</param>
        public TypedCacheWrapper(ICache internalCache)
        {
            InternalCache = internalCache;
        }

        public void Dispose()
        {
            InternalCache.Dispose();
        }

        public void Clear()
        {
            InternalCache.Clear();
        }

        public Task ClearAsync()
        {
            return InternalCache.ClearAsync();
        }

        public TValue Get(TKey key, Func<TKey, TValue> factory)
        {
            return InternalCache.Get(key, factory);
        }

        public TValue[] Get(TKey[] keys, Func<TKey, TValue> factory)
        {
            return InternalCache.Get(keys, factory);
        }

        public Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return InternalCache.GetAsync(key, factory);
        }

        public Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            return InternalCache.GetAsync(keys, factory);
        }

        public TValue GetOrDefault(TKey key)
        {
            return InternalCache.GetOrDefault<TKey, TValue>(key);
        }

        public TValue[] GetOrDefault(TKey[] keys)
        {
            return InternalCache.GetOrDefault<TKey, TValue>(keys);
        }

        public Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return InternalCache.GetOrDefaultAsync<TKey, TValue>(key);
        }

        public Task<TValue[]> GetOrDefaultAsync(TKey[] keys)
        {
            return InternalCache.GetOrDefaultAsync<TKey, TValue>(keys);
        }

        public void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            InternalCache.Set(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
        }

        public void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
            InternalCache.Set(stringPairs.ToArray(), slidingExpireTime, absoluteExpireTime);
        }

        public Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            return InternalCache.SetAsync(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
        }

        public Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
            return InternalCache.SetAsync(stringPairs.ToArray(), slidingExpireTime, absoluteExpireTime);
        }

        public void Remove(TKey key)
        {
            InternalCache.Remove(key.ToString());
        }

        public void Remove(TKey[] keys)
        {
            InternalCache.Remove(keys.Select(key => key.ToString()).ToArray());
        }

        public Task RemoveAsync(TKey key)
        {
            return InternalCache.RemoveAsync(key.ToString());
        }

        public Task RemoveAsync(TKey[] keys)
        {
            return InternalCache.RemoveAsync(keys.Select(key => key.ToString()).ToArray());
        }
    }
}