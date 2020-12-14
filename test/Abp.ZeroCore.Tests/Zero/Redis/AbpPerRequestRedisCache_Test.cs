using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Redis;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Shouldly;
using StackExchange.Redis;
using Xunit;

namespace Abp.Zero.Redis
{
    public class AbpPerRequestRedisCache_Test :  AbpIntegratedTestBase<AbpPerRequestRedisCacheTestModule>
    {
        private ITypedCache<string, MyCacheItem> _perRequestRedisCache;
        private ITypedCache<string, MyCacheItem> _normalRedisCache;

        private IDatabase _redisDatabase;
        private IRedisCacheSerializer _redisSerializer;

        private HttpContext _currentHttpContext;

        public AbpPerRequestRedisCache_Test()
        {
            _currentHttpContext = GetNewContextSubstitute();
            
            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(info => _currentHttpContext);

            LocalIocManager.IocContainer.Register(Component.For<IHttpContextAccessor>().Instance(httpContextAccessor).LifestyleSingleton().IsDefault());

            _redisDatabase = Substitute.For<IDatabase>();
            var redisDatabaseProvider = Substitute.For<IAbpRedisCacheDatabaseProvider>();

            redisDatabaseProvider.GetDatabase().Returns(_redisDatabase);

            LocalIocManager.IocContainer.Register(Component.For<IAbpRedisCacheDatabaseProvider>().Instance(redisDatabaseProvider).LifestyleSingleton().IsDefault());
            LocalIocManager.IocContainer.Register(Component.For<IAbpStartupConfiguration>().Instance(Substitute.For<IAbpStartupConfiguration>()).IsDefault());

            LocalIocManager.Resolve<ICachingConfiguration>().Configure("MyTestCacheItems", cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromHours(12); });
            LocalIocManager.Resolve<ICachingConfiguration>().Configure("MyPerRequestRedisTestCacheItems", cache => { cache.DefaultSlidingExpireTime = TimeSpan.FromHours(24); });
            
            _redisSerializer = LocalIocManager.Resolve<IRedisCacheSerializer>();

            _perRequestRedisCache = LocalIocManager.Resolve<IAbpPerRequestRedisCacheManager>().GetCache<string, MyCacheItem>("MyPerRequestRedisTestCacheItems");
            _normalRedisCache = LocalIocManager.Resolve<ICacheManager>().GetCache<string, MyCacheItem>("MyTestCacheItems");
        }

        private HttpContext GetNewContextSubstitute()
        {
            var httpContext = Substitute.For<HttpContext>();
            httpContext.Items = new Dictionary<object, object>();
            return httpContext;
        }

        [Fact]
        public void Cache_Options_Configuration_Test()
        {
            _normalRedisCache.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromHours(12));
            _perRequestRedisCache.DefaultSlidingExpireTime.ShouldBe(TimeSpan.FromHours(24));
        }

        [Fact]
        public void Should_Not_Change_Normal_Redis_Cache()
        {
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            MyCacheItem GetCacheValue()
            {
                counter++;
                return new MyCacheItem {Value = cacheValue};
            }

            _redisDatabase.StringSet(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan>()).Returns(true);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));

            var item = _normalRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(2).StringGet(Arg.Any<RedisKey>()); //redis cache tries to get value two times if value not exists see AbpCacheBase<TKey, TValue>.Get(TKey key, Func<TKey, TValue> factory)
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            _redisDatabase.StringGet(Arg.Any<RedisKey>()).Returns(cachedObject);

            var item1 = _normalRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(3).StringGet(Arg.Any<RedisKey>());

            var item2 = _normalRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(4).StringGet(Arg.Any<RedisKey>());

            //should still be one received calls
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);

            _normalRedisCache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);
        }

        [Theory]
        [InlineData("A", 42)]
        [InlineData("B", 43)]
        public void Simple_Get_Set_Test(string cacheKey, int cacheValue)
        {
            var item = _perRequestRedisCache.Get(cacheKey, () => new MyCacheItem {Value = cacheValue});

            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));
            _redisDatabase.StringGet(Arg.Any<RedisKey>()).Returns(cachedObject);
            _redisDatabase.Received().StringGet(Arg.Any<RedisKey>());

            _perRequestRedisCache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);
        }

        [Fact]
        public void Should_Request_Once_For_Same_Context()
        {
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            MyCacheItem GetCacheValue()
            {
                counter++;
                return new MyCacheItem {Value = cacheValue};
            }

            var item = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            var item1 = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            var item2 = _perRequestRedisCache.Get(cacheKey, GetCacheValue);

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);

            _redisDatabase.Received(1).StringGet(Arg.Any<RedisKey>());

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            _perRequestRedisCache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);
        }

        [Fact]
        public void Should_Request_Again_For_Different_Contexts()
        {
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            MyCacheItem GetCacheValue()
            {
                counter++;
                return new MyCacheItem {Value = cacheValue};
            }

            _redisDatabase.StringSet(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan>()).Returns(true);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));

            _currentHttpContext = GetNewContextSubstitute();

            var item = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(1).StringGet(Arg.Any<RedisKey>());
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            _redisDatabase.StringGet(Arg.Any<RedisKey>()).Returns(cachedObject);

            _currentHttpContext = GetNewContextSubstitute();
            var item1 = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(2).StringGet(Arg.Any<RedisKey>());

            _currentHttpContext = GetNewContextSubstitute();
            var item2 = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(3).StringGet(Arg.Any<RedisKey>());

            //should still be one received calls
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);
        }

        [Fact]
        public void Should_Work_With_Null_Contexts()
        {
            _currentHttpContext = null;
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            MyCacheItem GetCacheValue()
            {
                counter++;
                return new MyCacheItem {Value = cacheValue};
            }

            _redisDatabase.StringSet(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan>()).Returns(true);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));

            var item = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(2).StringGet(Arg.Any<RedisKey>());
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            _redisDatabase.StringGet(Arg.Any<RedisKey>()).Returns(cachedObject);

            var item1 = _perRequestRedisCache.Get(cacheKey, GetCacheValue);
            _redisDatabase.Received(3).StringGet(Arg.Any<RedisKey>()); //since _currentHttpContext is null it should go to the redisdb again

            var item2 = _perRequestRedisCache.Get(cacheKey, GetCacheValue); //since _currentHttpContext is null it should go to the redisdb again
            _redisDatabase.Received(4).StringGet(Arg.Any<RedisKey>());

            //should still be one received calls
            _redisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);
        }

        #region Async

        [Theory]
        [InlineData("A", 42)]
        [InlineData("B", 43)]
        public async Task Simple_Get_Set_Test_Async(string cacheKey, int cacheValue)
        {
            var item = await _perRequestRedisCache.GetAsync(cacheKey, () => Task.FromResult(new MyCacheItem {Value = cacheValue}));

            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));
            _redisDatabase.StringGetAsync(Arg.Any<RedisKey>()).Returns(Task.FromResult(cachedObject));
            await _redisDatabase.Received().StringGetAsync(Arg.Any<RedisKey>());

            (await _perRequestRedisCache.GetOrDefaultAsync(cacheKey)).Value.ShouldBe(cacheValue);
        }

        [Fact]
        public async Task Should_Request_Once_For_Same_Context_Async()
        {
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            Task<MyCacheItem> GetCacheValue()
            {
                counter++;
                return Task.FromResult(new MyCacheItem {Value = cacheValue});
            }

            var item = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            var item1 = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            var item2 = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);

            await _redisDatabase.Received(1).StringGetAsync(Arg.Any<RedisKey>());

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));
            await _redisDatabase.Received(1).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            (await _perRequestRedisCache.GetOrDefaultAsync(cacheKey)).Value.ShouldBe(cacheValue);
        }

        [Fact]
        public async Task Should_Request_Again_For_Different_Contexts_Async()
        {
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            Task<MyCacheItem> GetCacheValue()
            {
                counter++;
                return Task.FromResult(new MyCacheItem {Value = cacheValue});
            }

            _redisDatabase.StringSetAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan>()).Returns(true);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));

            _currentHttpContext = GetNewContextSubstitute();

            var item = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            await _redisDatabase.Received(1).StringGetAsync(Arg.Any<RedisKey>());
            await _redisDatabase.Received(1).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            _redisDatabase.StringGetAsync(Arg.Any<RedisKey>()).Returns(Task.FromResult(cachedObject));

            _currentHttpContext = GetNewContextSubstitute();
            var item1 = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            await _redisDatabase.Received(2).StringGetAsync(Arg.Any<RedisKey>());

            _currentHttpContext = GetNewContextSubstitute();
            var item2 = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            await _redisDatabase.Received(3).StringGetAsync(Arg.Any<RedisKey>());

            //should still be one received calls
            await _redisDatabase.Received(1).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);
        }

        [Fact]
        public async Task Should_Work_With_Null_Contexts_Async()
        {
            _currentHttpContext = null;
            _redisDatabase.ClearReceivedCalls();

            string cacheKey = "Test";
            int cacheValue = 1;

            int counter = 0;

            Task<MyCacheItem> GetCacheValue()
            {
                counter++;
                return Task.FromResult(new MyCacheItem {Value = cacheValue});
            }

            _redisDatabase.StringSetAsync(Arg.Any<RedisKey>(), Arg.Any<RedisValue>(), Arg.Any<TimeSpan>()).Returns(true);

            var cachedObject = _redisSerializer.Serialize(new MyCacheItem {Value = cacheValue}, typeof(MyCacheItem));

            var item = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            await _redisDatabase.Received(2).StringGetAsync(Arg.Any<RedisKey>());
            await _redisDatabase.Received(1).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            _redisDatabase.StringGetAsync(Arg.Any<RedisKey>()).Returns(Task.FromResult(cachedObject));

            var item1 = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue);
            await _redisDatabase.Received(3).StringGetAsync(Arg.Any<RedisKey>()); //since _currentHttpContext is null it should go to the redisdb again

            var item2 = await _perRequestRedisCache.GetAsync(cacheKey, GetCacheValue); //since _currentHttpContext is null it should go to the redisdb again
            await _redisDatabase.Received(4).StringGetAsync(Arg.Any<RedisKey>());

            //should still be one received calls
            await _redisDatabase.Received(1).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>(), Arg.Any<When>(), Arg.Any<CommandFlags>());

            counter.ShouldBe(1);
            item.ShouldNotBe(null);
            item.Value.ShouldBe(cacheValue);
            item1.ShouldNotBe(null);
            item1.Value.ShouldBe(cacheValue);
            item2.ShouldNotBe(null);
            item2.Value.ShouldBe(cacheValue);
        }

        #endregion

        [Serializable]
        public class MyCacheItem
        {
            public int Value { get; set; }
        }
    }
}