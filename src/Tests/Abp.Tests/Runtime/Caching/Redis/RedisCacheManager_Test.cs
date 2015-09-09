using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Xunit;
using Abp.RedisCache;
using Shouldly;
using Abp.Tests;
using Abp.RedisCache.Configuration;

namespace Abp.RedisCache.Tests
{
    public class RedisCacheManager_Test : TestBaseWithLocalIocManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly ITypedCache<string, MyCacheItem> _cache;
        public RedisCacheManager_Test()
        {
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<IAbpRedisConnectionProvider, AbpRedisConnectionProvider>();
            LocalIocManager.Register<ICacheManager, AbpRedisCacheManager>();
            _cacheManager = LocalIocManager.Resolve<ICacheManager>();

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMilliseconds(10);
            });

            _cache = _cacheManager.GetCache("MyCacheItems").AsTyped<string, MyCacheItem>();
           // _cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        [Fact]
        public void Simple_Get_Set_Test()
        {
            _cache.Clear();
            _cache.GetOrDefault("A").ShouldBe(null);

            _cache.Set("A", new MyCacheItem { Value = 42 });

            _cache.GetOrDefault("A").ShouldNotBe(null);
            _cache.GetOrDefault("A").Value.ShouldBe(42);
            _cache.Clear();
           
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }
    }
}
