using System;
using Abp.Configuration.Startup;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Redis;
using Abp.Tests;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Xunit;
using Shouldly;

namespace Abp.RedisCache.Tests
{
    public class RedisCacheManager_Test : TestBaseWithLocalIocManager
    {
        private readonly ITypedCache<string, MyCacheItem> _cache;

        public RedisCacheManager_Test()
        {
            LocalIocManager.Register<AbpRedisCacheOptions>();
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<IAbpRedisCacheDatabaseProvider, AbpRedisCacheDatabaseProvider>();
            LocalIocManager.Register<ICacheManager, AbpRedisCacheManager>();
            LocalIocManager.Register<IRedisCacheSerializer,DefaultRedisCacheSerializer>();
            LocalIocManager.IocContainer.Register(Component.For<IAbpStartupConfiguration>().Instance(Substitute.For<IAbpStartupConfiguration>()));

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().Configure("MyTestCacheItems", cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            _cache = LocalIocManager.Resolve<ICacheManager>().GetCache<string, MyCacheItem>("MyTestCacheItems");
            _cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        //[Theory]
        //[InlineData("A", 42)]
        //[InlineData("B", 43)]
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
    }
}
