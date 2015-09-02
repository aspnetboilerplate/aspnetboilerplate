using System;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace Abp.Runtime.Caching
{
    public abstract class CacheStoreBase<TKey, TValue> : ICacheStore<TKey, TValue>
    {
        public string Name { get; private set; }

        public TimeSpan DefaultSlidingExpireTime { get; set; }

        protected readonly object SyncObj = new object();

        private readonly AsyncLock _asyncLock = new AsyncLock();

        protected CacheStoreBase(string name, TimeSpan? defaultSlidingExpireTime)
        {
            Name = name;
            DefaultSlidingExpireTime = defaultSlidingExpireTime ?? TimeSpan.FromHours(1);
        }

        public abstract TValue GetOrDefault(TKey key);

        public virtual Task<TValue> GetOrDefaultAsync(TKey key)
        {
            return Task.FromResult(GetOrDefault(key));
        }

        public virtual TValue GetOrCreate(TKey key, Func<TValue> factory)
        {
            var cacheKey = key;
            var item = GetOrDefault(key);
            if (item == null)
            {
                lock (SyncObj)
                {
                    item = GetOrDefault(key);
                    if (item == null)
                    {
                        item = factory();
                        Set(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public virtual async Task<TValue> GetOrCreateAsync(TKey key, Func<Task<TValue>> factory)
        {
            var cacheKey = key;
            var item = await GetOrDefaultAsync(key);
            if (item == null)
            {
                using (await _asyncLock.LockAsync())
                {
                    item = await GetOrDefaultAsync(key);
                    if (item == null)
                    {
                        item = await factory();
                        await SetAsync(cacheKey, item);
                    }
                }
            }

            return item;
        }

        public abstract void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null);

        public virtual Task SetAsync(TKey key, TValue value, TimeSpan? slidingExpireTime = null)
        {
            Set(key, value, slidingExpireTime);
            return Task.FromResult(0);
        }

        public abstract void Remove(TKey key);

        public virtual Task RemoveAsync(TKey key)
        {
            Remove(key);
            return Task.FromResult(0);
        }

        public abstract void Clear();

        public Task ClearAsync()
        {
            Clear();
            return Task.FromResult(0);
        }

        public virtual void Dispose()
        {
            
        }
    }
}