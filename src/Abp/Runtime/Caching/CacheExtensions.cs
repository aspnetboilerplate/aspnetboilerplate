using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Extension methods for <see cref="ICache"/>.
    /// </summary>
    public static class CacheExtensions
    {
        public static object Get(this ICache cache, string key, Func<object> factory)
        {
            return cache.Get(key, k => factory());
        }

        public static Task<object> GetAsync(this ICache cache, string key, Func<Task<object>> factory)
        {
            return cache.GetAsync(key, k => factory());
        }

        public static ITypedCache<TKey, TValue> AsTyped<TKey, TValue>(this ICache cache)
        {
            return new TypedCacheWrapper<TKey, TValue>(cache);
        }
        
        public static TValue Get<TKey, TValue>(this ICache cache, TKey key, Func<TKey, TValue> factory)
        {
            return (TValue)cache.Get(key.ToString(), (k) => (object)factory(key));
        }

        public static TValue Get<TKey, TValue>(this ICache cache, TKey key, Func<TValue> factory)
        {
            return cache.Get(key, (k) => factory());
        }

        public static async Task<TValue> GetAsync<TKey, TValue>(this ICache cache, TKey key, Func<TKey, Task<TValue>> factory)
        {
            var value = await cache.GetAsync(key.ToString(), async (keyAsString) =>
            {
                var v = await factory(key);
                return (object)v;
            });

            return (TValue)value;
        }

        public static Task<TValue> GetAsync<TKey, TValue>(this ICache cache, TKey key, Func<Task<TValue>> factory)
        {
            return cache.GetAsync(key, (k) => factory());
        }

        public static TValue GetOrDefault<TKey, TValue>(this ICache cache, TKey key)
        {
            var value = cache.GetOrDefault(key.ToString());
            if (value == null)
            {
                return default(TValue);
            }

            return (TValue) value;
        }

        public static async Task<TValue> GetOrDefaultAsync<TKey, TValue>(this ICache cache, TKey key)
        {
            var value = await cache.GetOrDefaultAsync(key.ToString());
            if (value == null)
            {
                return default(TValue);
            }

            return (TValue)value;
        }
    }
}