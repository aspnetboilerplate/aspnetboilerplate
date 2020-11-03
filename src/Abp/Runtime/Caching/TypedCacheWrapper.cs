using Abp.Data;
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
        public DateTimeOffset? DefaultAbsoluteExpireTime
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
            return (TValue)InternalCache.Get(key.ToString(), (k) => factory(key));
        }

        public TValue[] Get(TKey[] keys, Func<TKey, TValue> factory)
        {
            var keysAsString = keys.Select((key) => key.ToString()).ToArray();
            var values = InternalCache.Get(keysAsString, (k) => factory((TKey)(k as object)));
            return values.Select(value => (TValue)value).ToArray();
        }

        public async Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            return (TValue)await InternalCache.GetAsync(key.ToString(), async (k) => await factory(key));
        }

        public async Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            var keysAsString = keys.Select((key) => key.ToString()).ToArray();
            var values = await InternalCache.GetAsync(keysAsString, async (k) => await factory((TKey)(k as object)));
            return values.Select(value => (TValue)value).ToArray();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var found = InternalCache.TryGetValue(key.ToString(), out object objectValue);
            value = CastOrDefault(objectValue);
            return found;
        }

        public async Task<ConditionalValue<TValue>> TryGetValueAsync(TKey key)
        {
            var result = await InternalCache.TryGetValueAsync(key.ToString());
            return CreateConditionalValue(result);
        }

        public ConditionalValue<TValue>[] TryGetValues(TKey[] keys)
        {
            var results = InternalCache.TryGetValues(keys.Select(key => key.ToString()).ToArray());
            return results.Select(CreateConditionalValue).ToArray();
        }

        public async Task<ConditionalValue<TValue>[]> TryGetValuesAsync(TKey[] keys)
        {
            var results = await InternalCache.TryGetValuesAsync(keys.Select(key => key.ToString()).ToArray());
            return results.Select(CreateConditionalValue).ToArray();
        }

        protected ConditionalValue<TValue> CreateConditionalValue(ConditionalValue<object> conditionalValue)
        {
            return new ConditionalValue<TValue>(conditionalValue.HasValue, CastOrDefault(conditionalValue.Value));
        }

        public TValue GetOrDefault(TKey key)
        {
            return CastOrDefault(InternalCache.GetOrDefault(key.ToString()));
        }

        public TValue[] GetOrDefault(TKey[] keys)
        {
            var keysAsString = keys.Select((key) => key.ToString()).ToArray();
            var values = InternalCache.GetOrDefault(keysAsString);
            return values.Select(CastOrDefault).ToArray();
        }

        public async Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return CastOrDefault(await InternalCache.GetOrDefaultAsync(key.ToString()));
        }

        public async Task<TValue[]> GetOrDefaultAsync(TKey[] keys)
        {
            var keysAsString = keys.Select((key) => key.ToString()).ToArray();
            var values = await InternalCache.GetOrDefaultAsync(keysAsString);
            return values.Select(CastOrDefault).ToArray();
        }

        private TValue CastOrDefault(object value)
        {
            return value == null ? default : (TValue)value;
        }

        public void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            InternalCache.Set(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
        }

        public void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            var stringPairs = pairs.Select(p => new KeyValuePair<string, object>(p.Key.ToString(), p.Value));
            InternalCache.Set(stringPairs.ToArray(), slidingExpireTime, absoluteExpireTime);
        }

        public Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            return InternalCache.SetAsync(key.ToString(), value, slidingExpireTime, absoluteExpireTime);
        }

        public Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
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