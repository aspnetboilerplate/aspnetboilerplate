using System;
using System.Runtime.Caching;

namespace Abp.Runtime.Caching.Memory
{
    public class MemoryCacheStore<TKey, TValue> : CacheStoreBase<TKey, TValue>
    {
        private MemoryCache _memoryCache;

        public MemoryCacheStore(string name)
            : base(name, null)
        {
            _memoryCache = new MemoryCache(Name);
        }


        public MemoryCacheStore(string name, TimeSpan defaultSlidingExpireTime)
            : base(name, defaultSlidingExpireTime)
        {
            _memoryCache = new MemoryCache(Name);
        }

        public override TValue GetOrDefault(TKey key)
        {
            return (TValue)_memoryCache.Get(key.ToString());
        }

        public override void Set(TKey key, TValue value, TimeSpan? slidingExpireTime = null)
        {
            //TODO: Optimize by using a default CacheItemPolicy?
            _memoryCache.Set(
                key.ToString(),
                value,
                new CacheItemPolicy
                {
                    SlidingExpiration = slidingExpireTime ?? DefaultSlidingExpireTime
                });
        }

        public override void Remove(TKey key)
        {
            _memoryCache.Remove(key.ToString());
        }

        public override void Clear()
        {
            _memoryCache.Dispose();
            _memoryCache = new MemoryCache(Name);
        }

        public override void Dispose()
        {
            _memoryCache.Dispose();
            base.Dispose();
        }
    }
}