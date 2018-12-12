using System;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Caching.Memory;

namespace Abp.Runtime.Caching.Redis.InMemory
{
    public class AbpRedisInMemoryCache : CacheBase, IAbpRedisInMemoryCache
    {
        private readonly AbpMemoryCache _memoryCache;
        private readonly AbpRedisCache _redisCache;

        public AbpRedisInMemoryCache(IIocManager iocManager, string name) : base(name)
        {
            _memoryCache = new AbpMemoryCache(name)
            {
                Logger = Logger
            }; 

            _redisCache = iocManager.Resolve<AbpRedisCache>(new { name });
        }

        public override object GetOrDefault(string key)
        {
            return _memoryCache.GetOrDefault(key);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            SetMemory(key, value, slidingExpireTime, absoluteExpireTime);
            _redisCache.Set(key, value, slidingExpireTime, absoluteExpireTime);
        }

        public override void Remove(string key)
        {
            _memoryCache.Remove(key);
            _redisCache.Remove(key);
        }

        public override void Clear()
        {
            _memoryCache.Clear();
            _redisCache.Clear();
        }


        public override void Dispose()
        {
            _memoryCache.Dispose();
            _redisCache.Dispose();
            base.Dispose();
        }

        private void SetMemory(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            _memoryCache.Set(key, value, slidingExpireTime, absoluteExpireTime);
        }

        public void SetMemoryOnly(string key, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            _memoryCache.Clear();
            var value = _redisCache.GetOrDefault(key);
            _memoryCache.Set(key, value, slidingExpireTime, absoluteExpireTime);
        }
    }
}
