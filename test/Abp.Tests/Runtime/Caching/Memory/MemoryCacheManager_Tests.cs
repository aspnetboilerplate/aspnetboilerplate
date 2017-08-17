using System;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Memory;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Tests.Runtime.Caching.Memory
{
    public class MemoryCacheManager_Tests : TestBaseWithLocalIocManager
    {
        private readonly ICacheManager _cacheManager;
        private readonly ITypedCache<string, MyCacheItem> _cache;

        public MemoryCacheManager_Tests()
        {
            LocalIocManager.Register<ICachingConfiguration, CachingConfiguration>();
            LocalIocManager.Register<ICacheManager, AbpMemoryCacheManager>();
            LocalIocManager.Register<MyClientPropertyInjects>(DependencyLifeStyle.Transient);
            LocalIocManager.IocContainer.Register(Component.For<IAbpStartupConfiguration>().Instance(Substitute.For<IAbpStartupConfiguration>()));

            _cacheManager = LocalIocManager.Resolve<ICacheManager>();

            var defaultSlidingExpireTime = TimeSpan.FromHours(24);
            LocalIocManager.Resolve<ICachingConfiguration>().ConfigureAll(cache =>
            {
                cache.DefaultSlidingExpireTime = defaultSlidingExpireTime;
            });

            _cache = _cacheManager.GetCache<string, MyCacheItem>("MyCacheItems");
            _cache.DefaultSlidingExpireTime.ShouldBe(defaultSlidingExpireTime);
        }

        [Fact]
        public void Simple_Get_Set_Test()
        {
            _cache.GetOrDefault("A").ShouldBe(null);

            _cache.Set("A", new MyCacheItem { Value = 42 });

            _cache.GetOrDefault("A").ShouldNotBe(null);
            _cache.GetOrDefault("A").Value.ShouldBe(42);

            _cache.Get("B", () => new MyCacheItem { Value = 43 }).Value.ShouldBe(43);
            _cache.Get("B", () => new MyCacheItem { Value = 44 }).Value.ShouldBe(43); //Does not call factory, so value is not changed
        }

        [Fact]
        public void MultiThreading_Test()
        {
            Parallel.For(
                0,
                2048,
                new ParallelOptions {MaxDegreeOfParallelism = 16},
                i =>
                {
                    var randomKey = RandomHelper.GetRandomOf("A", "B", "C", "D");
                    var randomValue = RandomHelper.GetRandom(0, 16);
                    switch (RandomHelper.GetRandom(0, 3))
                    {
                        case 0:
                            _cache.Get(randomKey, () => new MyCacheItem(randomValue));
                            _cache.GetOrDefault(randomKey);
                            break;
                        case 1:
                            _cache.GetOrDefault(randomKey);
                            _cache.Set(randomKey, new MyCacheItem(RandomHelper.GetRandom(0, 16)));
                            _cache.GetOrDefault(randomKey);
                            break;
                        case 2:
                            _cache.GetOrDefault(randomKey);
                            break;
                    }
                });
        }

        [Fact]
        public void Property_Injected_CacheManager_Should_Work()
        {
            LocalIocManager.Using<MyClientPropertyInjects>(client =>
            {
                client.SetGetValue(42).ShouldBe(42); //Not in cache, getting from factory
            });

            LocalIocManager.Using<MyClientPropertyInjects>(client =>
            {
                client.SetGetValue(43).ShouldBe(42); //Retrieving from the cache
            });
        }

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }

            public MyCacheItem()
            {
                
            }

            public MyCacheItem(int value)
            {
                Value = value;
            }
        }

        public class MyClientPropertyInjects
        {
            public ICacheManager CacheManager { get; set; }

            public int SetGetValue(int value)
            {
                return CacheManager
                    .GetCache("MyClientPropertyInjectsCache")
                    .Get("A", () =>
                    {
                        return value;
                    });
            }
        }
    }
}
