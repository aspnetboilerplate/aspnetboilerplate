using System;
using Abp.RedisCache.Configuration;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Tests;
using Xunit;
using Shouldly;

namespace Abp.RedisCache.Tests
{
    public class RedisCacheManager_Test : TestBaseWithLocalIocManager
    {
        private readonly ITypedCache<string, MyCacheItem> _cache;

        public RedisCacheManager_Test()
        {
            LocalIocManager.Register<AbpRedisCacheConfig, AbpRedisCacheConfig>();
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<IAbpRedisConnectionProvider, AbpRedisConnectionProvider>();
            LocalIocManager.Register<ICacheManager, AbpRedisCacheManager>();

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().Configure("MyCacheItems", cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            _cache = LocalIocManager.Resolve<ICacheManager>().GetCache<string, MyCacheItem>("MyCacheItems");
            _cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        //[Fact]
        public void Simple_Get_Set_Test()
        {
            _cache.GetOrDefault("A").ShouldBe(null);

            _cache.Set("A", new MyCacheItem { Value = 42 });

            _cache.GetOrDefault("A").ShouldNotBe(null);
            _cache.GetOrDefault("A").Value.ShouldBe(42);
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }
    }
}
