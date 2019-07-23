using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Nito.AsyncEx;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Base class for caches.
    /// It's used to simplify implementing <see cref="ICache"/>.
    /// </summary>
    public abstract class CacheBase : ICache
    {
        public ILogger Logger { get; set; }

        public string Name { get; }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected readonly object SyncObj = new object();

        private readonly AsyncLock _asyncLock = new AsyncLock();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name)
        {
            Name = name;
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);

            Logger = NullLogger.Instance;
        }

        public virtual object Get(string key, Func<string, object> factory)
        {
            object item = null;

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
                lock (SyncObj)
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
                            return null;
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

        public virtual object[] Get(string[] keys, Func<string, object> factory)
        {
            object[] items = null;

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
                items = new object[keys.Length];
            }

            if (items.Any(i => i == null))
            {
                lock (SyncObj)
                {
                    try
                    {
                        items = GetOrDefault(keys);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    var fetched = new List<KeyValuePair<string, object>>();
                    for (var i = 0; i < items.Length; i++)
                    {
                        string key = keys[i];
                        object value = items[i];
                        if (value == null)
                        {
                            value = factory(key);
                        }

                        if (value != null)
                        {
                            fetched.Add(new KeyValuePair<string, object>(key, value));
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

        public virtual async Task<object> GetAsync(string key, Func<string, Task<object>> factory)
        {
            object item = null;

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
                using (await _asyncLock.LockAsync())
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
                            return null;
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

        public virtual async Task<object[]> GetAsync(string[] keys, Func<string, Task<object>> factory)
        {
            object[] items = null;

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
                items = new object[keys.Length];
            }

            if (items.Any(i => i == null))
            {
                using (await _asyncLock.LockAsync())
                {
                    try
                    {
                        items = await GetOrDefaultAsync(keys);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString(), ex);
                    }

                    var fetched = new List<KeyValuePair<string, object>>();
                    for (var i = 0; i < items.Length; i++)
                    {
                        string key = keys[i];
                        object value = items[i];
                        if (value == null)
                        {
                            value = factory(key);
                        }

                        if (value != null)
                        {
                            fetched.Add(new KeyValuePair<string, object>(key, value));
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

        public abstract object GetOrDefault(string key);

        public virtual object[] GetOrDefault(string[] keys)
        {
            return keys.Select(GetOrDefault).ToArray();
        }

        public virtual Task<object> GetOrDefaultAsync(string key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        public virtual Task<object[]> GetOrDefaultAsync(string[] keys)
        {
            return Task.FromResult(GetOrDefault(keys));
        }

        public abstract void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        public virtual void Set(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            foreach (var pair in pairs)
            {
                Set(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
            }
        }

        public virtual Task SetAsync(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            Set(key, value, slidingExpireTime, absoluteExpireTime);
            return Task.FromResult(0);
        }

        public virtual Task SetAsync(KeyValuePair<string, object>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            return Task.WhenAll(pairs.Select(p => SetAsync(p.Key, p.Value, slidingExpireTime, absoluteExpireTime)));
        }

        public abstract void Remove(string key);

        public virtual void Remove(string[] keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public virtual Task RemoveAsync(string key)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public virtual Task RemoveAsync(string[] keys)
        {
            return Task.WhenAll(keys.Select(RemoveAsync));
        }

        public abstract void Clear();

        public virtual Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {

        }
    }

    /// <summary>
    /// Base class for caches with multiple keys operations.
    /// </summary>
    public abstract class CacheBase<TKey, TSingleValue, TMultipleValues> : CacheBase<TKey, TSingleValue>, ICacheMultipleKeysOperations<TKey, TSingleValue, TMultipleValues>
    {
        protected CacheBase(string name) : base(name)
        {
        }

        public abstract TMultipleValues Get(TKey[] keys, Func<TKey, TSingleValue> factory);

        public abstract Task<TMultipleValues> GetAsync(TKey[] keys, Func<TKey, Task<TSingleValue>> factory);

        public abstract TMultipleValues GetOrDefault(TKey[] keys);

        public abstract Task<TMultipleValues> GetOrDefaultAsync(TKey[] keys);

        public virtual void Set(KeyValuePair<TKey, TSingleValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            foreach (var pair in pairs)
            {
                Set(pair.Key, pair.Value, slidingExpireTime, absoluteExpireTime);
            }
        }

        public virtual Task SetAsync(KeyValuePair<TKey, TSingleValue>[] pairs, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            return Task.WhenAll(pairs.Select(p => SetAsync(p.Key, p.Value, slidingExpireTime, absoluteExpireTime)));
        }

        public virtual void Remove(TKey[] keys)
        {
            foreach (var key in keys)
            {
                Remove(key);
            }
        }

        public virtual Task RemoveAsync(TKey[] keys)
        {
            return Task.WhenAll(keys.Select(RemoveAsync));
        }
    }

    /// <summary>
    /// Base class for caches with single key operations.
    /// </summary>
    public abstract class CacheBase<TKey, TValue> : IDisposable, ICacheOptions, ICacheOperations, ICacheSingleKeyOperations<TKey, TValue>
    {
        public ILogger Logger { get; set; }

        public string Name { get; }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        public TimeSpan? DefaultAbsoluteExpireTime { get; set; }

        protected readonly object SyncObj = new object();

        protected readonly AsyncLock AsyncLock = new AsyncLock();

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name"></param>
        protected CacheBase(string name)
        {
            Name = name;
            DefaultSlidingExpireTime = TimeSpan.FromHours(1);

            Logger = NullLogger.Instance;
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
                lock (SyncObj)
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

        public abstract TValue GetOrDefault(TKey key);

        public virtual Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        public abstract void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null);

        public virtual Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            Set(key, value, slidingExpireTime, absoluteExpireTime);
            return Task.CompletedTask;
        }

        public abstract void Remove(TKey key);

        public virtual Task RemoveAsync(TKey key)
        {
            Remove(key);
            return Task.CompletedTask;
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
