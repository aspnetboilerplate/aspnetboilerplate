using System;
using System.Linq;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Extension methods for <see cref="ICache"/>.
    /// </summary>
    public static class CacheExtensions
    {
        public static ITypedCache<TKey, TValue> AsTyped<TKey, TValue>(this ICache cache)
        {
            return new TypedCacheWrapper<TKey, TValue>(cache);
        }

        [Obsolete]
        public static object Get(this ICache cache, string key, Func<object> factory)
        {
            return cache.Get(key, k => factory());
        }

        [Obsolete]
        public static object[] Get(this ICache cache, string[] keys, Func<object> factory)
        {
            return keys.Select(key => cache.Get(key, k => factory())).ToArray();
        }

        [Obsolete]
        public static Task<object> GetAsync(this ICache cache, string key, Func<Task<object>> factory)
        {
            return cache.GetAsync(key, k => factory());
        }

        [Obsolete]
        public static Task<object[]> GetAsync(this ICache cache, string[] keys, Func<Task<object>> factory)
        {
            var tasks = keys.Select(key => cache.GetAsync(key, k => factory()));
            return Task.WhenAll(tasks);
        }

        [Obsolete]
        public static TValue Get<TKey, TValue>(this ICache cache, TKey key, Func<TKey, TValue> factory)
        {
            return (TValue)cache.Get(key.ToString(), (k) => (object)factory(key));
        }

        [Obsolete]
        public static TValue[] Get<TKey, TValue>(this ICache cache, TKey[] keys, Func<TKey, TValue> factory)
        {
            return keys.Select(key => (TValue)cache.Get(key.ToString(), (k) => (object)factory(key))).ToArray();
        }

        [Obsolete]
        public static TValue Get<TKey, TValue>(this ICache cache, TKey key, Func<TValue> factory)
        {
            return cache.Get(key, (k) => factory());
        }

        [Obsolete]
        public static TValue[] Get<TKey, TValue>(this ICache cache, TKey[] keys, Func<TValue> factory)
        {
            return keys.Select(key => cache.Get(key, (k) => factory())).ToArray();
        }

        [Obsolete]
        public static async Task<TValue> GetAsync<TKey, TValue>(this ICache cache, TKey key, Func<TKey, Task<TValue>> factory)
        {
            var value = await cache.GetAsync(key.ToString(), async (keyAsString) =>
            {
                var v = await factory(key);
                return (object)v;
            });

            return (TValue)value;
        }

        [Obsolete]
        public static async Task<TValue[]> GetAsync<TKey, TValue>(this ICache cache, TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            var tasks = keys.Select(key =>
            {
                return cache.GetAsync(key.ToString(), async (keyAsString) =>
                {
                    var v = await factory(key);
                    return (object)v;
                });
            });
            var values = await Task.WhenAll(tasks);
            return values.Select(value => (TValue)value).ToArray();
        }

        [Obsolete]
        public static Task<TValue> GetAsync<TKey, TValue>(this ICache cache, TKey key, Func<Task<TValue>> factory)
        {
            return cache.GetAsync(key, (k) => factory());
        }

        [Obsolete]
        public static Task<TValue[]> GetAsync<TKey, TValue>(this ICache cache, TKey[] keys, Func<Task<TValue>> factory)
        {
            var tasks = keys.Select(key => cache.GetAsync(key, (k) => factory()));
            return Task.WhenAll(tasks);
        }

        [Obsolete]
        public static TValue GetOrDefault<TKey, TValue>(this ICache cache, TKey key)
        {
            var value = cache.GetOrDefault(key.ToString());
            if (value == null)
            {
                return default(TValue);
            }

            return (TValue)value;
        }

        [Obsolete]
        public static TValue[] GetOrDefault<TKey, TValue>(this ICache cache, TKey[] keys)
        {
            var values = keys.Select(key =>
            {
                var value = cache.GetOrDefault(key.ToString());
                if (value == null)
                {
                    return default(TValue);
                }
                return (TValue)value;
            });

            return values.ToArray();
        }

        [Obsolete]
        public static async Task<TValue> GetOrDefaultAsync<TKey, TValue>(this ICache cache, TKey key)
        {
            var value = await cache.GetOrDefaultAsync(key.ToString());
            if (value == null)
            {
                return default(TValue);
            }

            return (TValue)value;
        }

        [Obsolete]
        public static async Task<TValue[]> GetOrDefaultAsync<TKey, TValue>(this ICache cache, TKey[] keys)
        {
            var tasks = keys.Select(async (key) =>
            {
                var value = await cache.GetOrDefaultAsync(key.ToString());
                if (value == null)
                {
                    return default(TValue);
                }
                return (TValue)value;
            });

            return await Task.WhenAll(tasks);
        }
    }
}
