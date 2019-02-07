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

            this.Logger.Debug($"GetOrDefault|{this.Name}|{key}");

            return _memoryCache.GetOrDefault(key);
        }

        public override void Set(string key, object value, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            this.Logger.Debug($"Set|{this.Name}|{key}");
            
            var memorySlidingExpireTime = slidingExpireTime ?? _redisCache.DefaultSlidingExpireTime.Add(new TimeSpan(0, 1, 0));

            _memoryCache.Set(key, value, memorySlidingExpireTime, absoluteExpireTime);

            _redisCache.Set(key, value, slidingExpireTime, absoluteExpireTime);           
        }

        public override void Remove(string key)
        {
            this.Logger.Debug($"Remove|{this.Name}|{key}");
            _memoryCache.Remove(key);
            _redisCache.Remove(key);
        }

        public void RemoveMemoryOnly(string key)
        {
            this.Logger.Debug($"RemoveMemoryOnly|{this.Name}|{key}");
            _memoryCache.Remove(key);
        }

        public override void Clear()
        {
            this.Logger.Debug($"Clear|{this.Name}");
            _memoryCache.Clear();
            _redisCache.Clear();
        }


        public override void Dispose()
        {
            this.Logger.Debug($"Dispose|{this.Name}");
            _memoryCache.Dispose();
            _redisCache.Dispose();
            base.Dispose();
        }


        public void SetMemoryOnly(string key, TimeSpan? slidingExpireTime = null, TimeSpan? absoluteExpireTime = null)
        {
            this.Logger.Debug($"SetMemoryOnly|{this.Name}|{key}");
            //this.Logger.Debug($"\tMemCache Key removed");
            _memoryCache.Remove(key);

            //this.Logger.Debug($"\tRedis GetOrDefault");
            var value = _redisCache.GetOrDefault(key);

            //this.Logger.Debug($"\tMemCache Key Set");
            if (value != null)
            {
                _memoryCache.Set(key, value, slidingExpireTime, absoluteExpireTime);
            }
        }
    }
}
