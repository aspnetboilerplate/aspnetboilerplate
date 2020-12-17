using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using Abp.Data;
using Abp.Threading.Extensions;
using Castle.Core.Logging;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Base class for caches with generic types, <see cref="AbpCacheBase{TKey, TValue}"/> and .
    /// It provides default implementation of <see cref="IAbpCache{TKey, TValue}"/>.
    /// </summary>
    public abstract class AbpCacheBase<TKey, TValue> : AbpCacheBase, IAbpCache<TKey, TValue>, ICacheOptions
    {
        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public DateTimeOffset? DefaultAbsoluteExpireTime { get; set; }

        protected readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1, 1);

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
            if (TryGetValue(key, out TValue value))
            {
                return value;
            }

            using (SemaphoreSlim.Lock())
            {
                if (TryGetValue(key, out value))
                {
                    return value;
                }

                var generatedValue = factory(key);
                if (!IsDefaultValue(generatedValue))
                {
                    try
                    {
                        Set(key, generatedValue);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }
                }
                return generatedValue;
            }
        }

        public virtual TValue[] Get(TKey[] keys, Func<TKey, TValue> factory)
        {
            ConditionalValue<TValue>[] results = null;
            try
            {
                results = TryGetValues(keys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (results == null)
            {
                results = new ConditionalValue<TValue>[keys.Length];
            }

            if (results.Any(result => !result.HasValue))
            {
                using (SemaphoreSlim.Lock())
                {
                    try
                    {
                        results = TryGetValues(keys);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    var generated = new List<KeyValuePair<TKey, TValue>>();
                    for (var i = 0; i < results.Length; i++)
                    {
                        var result = results[i];
                        if (!result.HasValue)
                        {
                            var key = keys[i];
                            var generatedValue = factory(key);
                            results[i] = new ConditionalValue<TValue>(true, generatedValue);

                            if (!IsDefaultValue(generatedValue))
                            {
                                generated.Add(new KeyValuePair<TKey, TValue>(key, generatedValue));
                            }
                        }
                    }

                    if (generated.Any())
                    {
                        try
                        {
                            Set(generated.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return results.Select(result => result.Value).ToArray();
        }

        protected virtual bool IsDefaultValue(TValue value)
        {
            return EqualityComparer<TValue>.Default.Equals(value, default);
        }

        public virtual async Task<TValue> GetAsync(TKey key, Func<TKey, Task<TValue>> factory)
        {
            ConditionalValue<TValue> result = default;

            try
            {
                result = await TryGetValueAsync(key);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (result.HasValue)
            {
                return result.Value;
            }

            using (await SemaphoreSlim.LockAsync())
            {
                try
                {
                    result = await TryGetValueAsync(key);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }

                if (result.HasValue)
                {
                    return result.Value;
                }

                var generatedValue = await factory(key);
                if (IsDefaultValue(generatedValue))
                {
                    return generatedValue;
                }

                try
                {
                    await SetAsync(key, generatedValue);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.ToString(), ex);
                }

                return generatedValue;
            }
        }

        public virtual async Task<TValue[]> GetAsync(TKey[] keys, Func<TKey, Task<TValue>> factory)
        {
            ConditionalValue<TValue>[] results = null;

            try
            {
                results = await TryGetValuesAsync(keys);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.ToString(), ex);
            }

            if (results == null)
            {
                results = new ConditionalValue<TValue>[keys.Length];
            }

            if (results.Any(result => !result.HasValue))
            {
                using (await SemaphoreSlim.LockAsync())
                {
                    try
                    {
                        results = await TryGetValuesAsync(keys);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    var generated = new List<KeyValuePair<TKey, TValue>>();
                    for (var i = 0; i < results.Length; i++)
                    {
                        var result = results[i];
                        if (!result.HasValue)
                        {
                            var key = keys[i];
                            var generatedValue = await factory(key);
                            if (!IsDefaultValue(generatedValue))
                            {
                                generated.Add(new KeyValuePair<TKey, TValue>(key, generatedValue));
                            }
                            results[i] = new ConditionalValue<TValue>(true, generatedValue);
                        }
                    }

                    if (generated.Any())
                    {
                        try
                        {
                            await SetAsync(generated.ToArray());
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex.ToString(), ex);
                        }
                    }
                }
            }

            return results.Select(result => result.Value).ToArray();
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

        public abstract void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null);

        public virtual void Set(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            foreach (var pair in pairs)
            {
                Set(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
            }
        }

        public virtual Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
        {
            Set(key, value, slidingExpireTime, absoluteExpireTime);
            return Task.CompletedTask;
        }

        public virtual Task SetAsync(KeyValuePair<TKey, TValue>[] pairs, TimeSpan? slidingExpireTime = null, DateTimeOffset? absoluteExpireTime = null)
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
