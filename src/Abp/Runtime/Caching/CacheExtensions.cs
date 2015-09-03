using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Extension methods for cachine.
    /// </summary>
    public static class CacheExtensions
    {
        public static ITypedCache<TKey, TValue> AsTyped<TKey, TValue>(this ICache cache)
        {
            return new TypedCacheWrapper<TKey, TValue>(cache);
        }

        public static TValue Get<TKey, TValue>(this ICache cache, TKey key, Func<TValue> factory)
        {
            return (TValue)cache.Get(key.ToString(), () => (object)factory());
        }

        public static async Task<TValue> GetAsync<TKey, TValue>(this ICache cache, TKey key, Func<Task<TValue>> factory)
        {
            var value = await cache.GetAsync(key.ToString(), async () =>
            {
                var v = await factory();
                return (object)v;
            });

            return (TValue)value;
        }

        public static TValue GetOrDefault<TKey, TValue>(this ICache cache, TKey key)
        {
            return (TValue)cache.GetOrDefault(key.ToString());
        }

        public static async Task<TValue> GetOrDefaultAsync<TKey, TValue>(this ICache cache, TKey key)
        {
            var value = await cache.GetOrDefaultAsync(key.ToString());
            return (TValue)value;
        }
    }
}