using System;
using Adorable.RedisCache.Configuration;
using Adorable.Runtime.Caching;
using Adorable.Runtime.Caching.Configuration;
using Adorable.Tests;
using Xunit;
using Shouldly;

namespace Adorable.RedisCache.Tests
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
            LocalIocManager.Resolve<ICachingConfiguration>().Configure("MyTestCacheItems", cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            _cache = LocalIocManager.Resolve<ICacheManager>().GetCache<string, MyCacheItem>("MyTestCacheItems");
            _cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        [Theory]
        [InlineData("A", 42)]
        [InlineData("B", 43)]
        public void Simple_Get_Set_Test(string cacheKey, int cacheValue)
        {
            var item = _cache.Get(cacheKey, () => new MyCacheItem { Value = cacheValue });

            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);

            _cache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }

        [Fact]
        public void DatabaseId_Test()
        {
            var dbIdAppSettingName = LocalIocManager.Resolve<AbpRedisCacheConfig>().DatabaseIdAppSetting;

            var dbIdInConfig = LocalIocManager.Resolve<IAbpRedisConnectionProvider>().GetDatabaseId(dbIdAppSettingName);

            ((AbpRedisCache)_cache.InternalCache).DatabaseId.ShouldBe(dbIdInConfig);
        }
    }
}
