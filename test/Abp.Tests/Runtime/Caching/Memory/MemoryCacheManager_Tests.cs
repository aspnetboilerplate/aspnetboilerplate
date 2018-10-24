using System;
using System.Collections.Generic;
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

            var items1 = _cache.GetOrDefault(new string[] { "B", "C" });
            items1[0].Value.ShouldBe(43);
            items1[1].ShouldBeNull();

            var items2 = _cache.GetOrDefault(new string[] { "C", "D" });
            items2[0].ShouldBeNull();
            items2[1].ShouldBeNull();

            _cache.Set(new KeyValuePair<string, MyCacheItem>[] {
                new KeyValuePair<string, MyCacheItem>("C", new MyCacheItem{ Value = 44}),
                new KeyValuePair<string, MyCacheItem>("D", new MyCacheItem{ Value = 45})
            });

            var items3 = _cache.GetOrDefault(new string[] { "C", "D" });
            items3[0].Value.ShouldBe(44);
            items3[1].Value.ShouldBe(45);

            var items4 = _cache.Get(new string[] { "D", "E" }, (key) => new MyCacheItem { Value = key == "D" ? 46 : 47 });
            items4[0].Value.ShouldBe(45); //Does not call factory, so value is not changed
            items4[1].Value.ShouldBe(47);
        }

        [Fact]
        public void MultiThreading_Test()
        {
            Parallel.For(
                0,
                2048,
                new ParallelOptions { MaxDegreeOfParallelism = 16 },
                i =>
                {
                    var randomKey = RandomHelper.GetRandomOf("A", "B", "C", "D");
                    var randomValue = RandomHelper.GetRandom(0, 32);
                    switch (RandomHelper.GetRandom(0, 4))
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
                        case 3:
                            var randomKeys = new string[] { randomKey, randomKey + randomKey };
                            _cache.GetOrDefault(randomKeys);
                            var pairs = new KeyValuePair<string, MyCacheItem>[]
                            {
                                new KeyValuePair<string, MyCacheItem>(randomKeys[0], new MyCacheItem{ Value= RandomHelper.GetRandom(0, 16) }),
                                new KeyValuePair<string, MyCacheItem>(randomKeys[1], new MyCacheItem{ Value= RandomHelper.GetRandom(0, 16) })
                            };
                            _cache.Set(pairs);
                            _cache.GetOrDefault(randomKeys);
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
