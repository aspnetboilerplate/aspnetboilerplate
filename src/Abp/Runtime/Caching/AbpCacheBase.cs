using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Abp.Data;
using Castle.Core.Logging;
using Nito.AsyncEx;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Base class for caches with generic types, <see cref="AbpCacheBase{TKey, TValue}"/> and .
    /// It provides default implementation of <see cref="IAbpCache{TKey, TValue}"/>.
    /// </summary>
    public abstract class AbpCacheBase<TKey, TValue> : AbpCacheBase, IAbpCache<TKey, TValue>, ICacheOptions
    {
        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected readonly AsyncLock AsyncLock = new AsyncLock();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected AbpCacheBase(string name) : base(name)
        {
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);
        }

        public virtual TValue Get(TKey key, Func<TKey, TValue> factory)
        {
            TValue item = default(TValue);

            try
            {
                item = GetOrDefault(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (item == null)
            {
                using (AsyncLock.Lock())
                {
                    try
                    {
                        item = GetOrDefault(key);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    if (item == null)
                    {
                        item = factory(key);

                        if (item == null)
                        {
                            return default(TValue);
                        }

                        try
                        {
                            Set(key, item);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return item;
        }

        public virtual TValue[] Get(TKey[] keys, Func<TKey, TValue> factory)
        {
            TValue[] items = null;

            try
            {
                items = GetOrDefault(keys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (items == null)
            {
                items = new TValue[keys.Length];
            }

            if (items.Any(i => i == null))
            {
                using (AsyncLock.Lock())
                {
                    try
                    {
                        items = GetOrDefault(keys);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    var fetched = new List<KeyValuePair<TKey, TValue>>();
                    for (var i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null)
                        {
                            var key = keys[i];
                            var item = items[i] = factory(key);
                            if (item != null)
                            {
                                fetched.Add(new KeyValuePair<TKey, TValue>(key, item));
                            }
                        }
                    }

                    if (fetched.Any())
                    {
                        try
                        {
                            Set(fetched.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return items;
        }

        public virtual async Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            TValue item = default(TValue);

            try
            {
                item = await GetOrDefaultAsync(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (item == null)
            {
                using (await AsyncLock.LockAsync())
                {
                    try
                    {
                        item = await GetOrDefaultAsync(key);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    if (item == null)
                    {
                        item = await factory(key);

                        if (item == null)
                        {
                            return default(TValue);
                        }

                        try
                        {
                            await SetAsync(key, item);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return item;
        }

        public virtual async Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            TValue[] items = null;

            try
            {
                items = await GetOrDefaultAsync(keys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (items == null)
            {
                items = new TValue[keys.Length];
            }

            if (items.Any(i => i == null))
            {
                using (await AsyncLock.LockAsync())
                {
                    try
                    {
                        items = await GetOrDefaultAsync(keys);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    var fetched = new List<KeyValuePair<TKey, TValue>>();
                    for (var i = 0; i < items.Length; i++)
                    {
                        if (items[i] == null)
                        {
                            var key = keys[i];
                            var item = items[i] = await factory(key);
                            if (item != null)
                            {
                                fetched.Add(new KeyValuePair<TKey, TValue>(key, item));
                            }
                        }
                    }

                    if (fetched.Any())
                    {
                        try
                        {
                            await SetAsync(fetched.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return items;
        }

        public abstract bool TryGetValue(TKey key, out TValue value);

        public virtual ConditionalValue<TValue>[] TryGetValues(TKey[] keys)
        {
            var pairs = new List<ConditionalValue<TValue>>();
            foreach (var key in keys)
            {
                var found = TryGetValue(key, out TValue value);
                pairs.Add(new ConditionalValue<TValue>(found, value));
            }
            return pairs.ToArray();
        }

        public virtual Task<ConditionalValue<TValue>> TryGetValueAsync(TKey key)
        {
            var found = TryGetValue(key, out TValue value);
            return Task.FromResult(new ConditionalValue<TValue>(found, value));
        }

        public virtual Task<ConditionalValue<TValue>[]> TryGetValuesAsync(TKey[] keys)
        {
            return Task.FromResult(TryGetValues(keys));
        }

        public virtual TValue GetOrDefault(TKey key)
        {
            TryGetValue(key, out TValue value);
            return value;
        }

        public virtual TValue[] GetOrDefault(TKey[] keys)
        {
            var results = TryGetValues(keys);
            return results.Select(result => result.Value).ToArray();
        }

        public virtual async Task<TValue> GetOrDefaultAsync(TKey key)
        {
            var result = await TryGetValueAsync(key);
            return result.Value;
        }

        public virtual async Task<TValue[]> GetOrDefaultAsync(TKey[] keys)
        {
            var results = await TryGetValuesAsync(keys);
            return results.Select(result => result.Value).ToArray();
        }

        public abstract void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        public virtual void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            foreach (var pair in pairs)
            {
                Set(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
            }
        }

        public virtual Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            Set(key, value, slidingExpireTime, absoluteExpireTime);
            return Task.CompletedTask;
        }

        public virtual Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            return Task.WhenAll(pairs.Select(p => SetAsync(p.Key, p.Value, slidingExpireTime, absoluteExpireTime)));
        }

        public abstract void Remove(TKey key);

        public virtual void Remove(TKey[] keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public virtual Task RemoveAsync(TKey key)
        {
            Remove(key);
            return Task.CompletedTask;
        }

        public virtual Task RemoveAsync(TKey[] keys)
        {
            return Task.WhenAll(keys.Select(RemoveAsync));
        }
    }

    /// <summary>
    /// Base class for caches.
    /// </summary>
    public abstract class AbpCacheBase : IDisposable, IAbpCache
    {
        public ILogger Logger { get; set; }

        public string Name { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected AbpCacheBase(string name)
        {
            Name = name;

            Logger = NullLogger.Instance;
        }

        public abstract void Clear();

        public virtual Task ClearAsync()
        {
            Clear();
            return Task.CompletedTask;
        }

        public virtual void Dispose()
        {
        }
    }
}
