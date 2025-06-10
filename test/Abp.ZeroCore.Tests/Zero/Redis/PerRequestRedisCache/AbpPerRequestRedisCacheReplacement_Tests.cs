using System;
using System.Threading.Tasks;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Redis;
using Microsoft.Extensions.Options;
using NSubstitute;
using Shouldly;
using StackExchange.Redis;
using Xunit;

namespace Abp.Zero.Redis.PerRequestRedisCache;

public class
    AbpPerRequestRedisCacheReplacement_Tests : PerRequestRedisCacheTestsBase<
        AbpPerRequestRedisCacheReplacementModule>
{
    private ITypedCache<string, TestCacheItem> _typedCache;

    public AbpPerRequestRedisCacheReplacement_Tests()
    {
        LocalIocManager.Register<IOptions<AbpRedisCacheOptions>, OptionsWrapper<AbpRedisCacheOptions>>();
        _typedCache = LocalIocManager.Resolve<ICacheManager>().GetCache<string, TestCacheItem>("TestCacheItems");
    }

    [Fact]
    public void Should_Request_Once_For_Same_Context()
    {
        ChangeHttpContext();

        string cacheKey = Guid.NewGuid().ToString();
        int counter = 0;
        int cacheValue = 1;

        TestCacheItem GetCacheValue()
        {
            counter++;
            return new TestCacheItem { Value = cacheValue };
        }

        var item1 = _typedCache.Get(cacheKey, GetCacheValue);
        var item2 = _typedCache.Get(cacheKey, GetCacheValue);
        RedisDatabase.Received(1).StringGet(Arg.Any<RedisKey>());

        var cachedObject =
            RedisSerializer.Serialize(new TestCacheItem { Value = cacheValue }, typeof(TestCacheItem));
        RedisDatabase.Received(1).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>());

        _typedCache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);

        counter.ShouldBe(1);

        item1.ShouldNotBe(null);
        item1.Value.ShouldBe(cacheValue);

        item2.ShouldNotBe(null);
        item2.Value.ShouldBe(cacheValue);
    }

    [Fact]
    public void Should_Request_Again_For_Same_Context()
    {
        ChangeHttpContext();

        string cacheKey = Guid.NewGuid().ToString();
        int counter = 0;
        int cacheValue = 1;

        TestCacheItem GetCacheValue()
        {
            counter++;
            return new TestCacheItem { Value = cacheValue };
        }

        var item1 = _typedCache.Get(cacheKey, GetCacheValue);

        ChangeHttpContext();
        var item2 = _typedCache.Get(cacheKey, GetCacheValue);

        RedisDatabase.Received(2).StringGet(Arg.Any<RedisKey>());

        var cachedObject =
            RedisSerializer.Serialize(new TestCacheItem { Value = cacheValue }, typeof(TestCacheItem));
        RedisDatabase.Received(2).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>());

        _typedCache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);

        counter.ShouldBe(2);

        item1.ShouldNotBe(null);
        item1.Value.ShouldBe(cacheValue);

        item2.ShouldNotBe(null);
        item2.Value.ShouldBe(cacheValue);
    }

    [Fact]
    public void Should_Not_Request_Again_For_Same_Context()
    {
        var context1 = GetNewContextSubstitute();
        var context2 = GetNewContextSubstitute();

        string cacheKey = Guid.NewGuid().ToString();
        int counter = 0;
        int cacheValue = 1;

        TestCacheItem GetCacheValue()
        {
            counter++;
            return new TestCacheItem { Value = cacheValue };
        }

        CurrentHttpContext = context1;
        var item1 = _typedCache.Get(cacheKey, GetCacheValue); //First request

        CurrentHttpContext = context2;
        var item2 = _typedCache.Get(cacheKey, GetCacheValue); //Second request

        CurrentHttpContext = context1;
        var item3 = _typedCache.Get(cacheKey, GetCacheValue); //First request again

        CurrentHttpContext = context2;
        var item4 = _typedCache.Get(cacheKey, GetCacheValue); //Second request again

        RedisDatabase.Received(2).StringGet(Arg.Any<RedisKey>());

        var cachedObject = RedisSerializer.Serialize(new TestCacheItem { Value = cacheValue }, typeof(TestCacheItem));
        RedisDatabase.Received(2).StringSet(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>());

        _typedCache.GetOrDefault(cacheKey).Value.ShouldBe(cacheValue);

        counter.ShouldBe(2);
        item1.ShouldNotBe(null);
        item1.Value.ShouldBe(cacheValue);

        item2.ShouldNotBe(null);
        item2.Value.ShouldBe(cacheValue);

        item3.ShouldNotBe(null);
        item3.Value.ShouldBe(cacheValue);

        item4.ShouldNotBe(null);
        item4.Value.ShouldBe(cacheValue);
    }

    #region Async Methods

    [Fact]
    public async Task Should_Request_Once_For_Same_Context_Async()
    {
        ChangeHttpContext();

        string cacheKey = Guid.NewGuid().ToString();
        int counter = 0;
        int cacheValue = 1;

        Task<TestCacheItem> GetCacheValue()
        {
            counter++;
            return Task.FromResult(new TestCacheItem { Value = cacheValue });
        }

        var item1 = await _typedCache.GetAsync(cacheKey, GetCacheValue);
        var item2 = await _typedCache.GetAsync(cacheKey, GetCacheValue);
        await RedisDatabase.Received(1).StringGetAsync(Arg.Any<RedisKey>());

        var cachedObject =
            RedisSerializer.Serialize(new TestCacheItem { Value = cacheValue }, typeof(TestCacheItem));
        await RedisDatabase.Received(1).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>());

        (await _typedCache.GetOrDefaultAsync(cacheKey)).Value.ShouldBe(cacheValue);

        counter.ShouldBe(1);

        item1.ShouldNotBe(null);
        item1.Value.ShouldBe(cacheValue);

        item2.ShouldNotBe(null);
        item2.Value.ShouldBe(cacheValue);
    }

    [Fact]
    public async Task Should_Request_Again_For_Same_Context_Async()
    {
        ChangeHttpContext();

        string cacheKey = Guid.NewGuid().ToString();
        int counter = 0;
        int cacheValue = 1;

        Task<TestCacheItem> GetCacheValue()
        {
            counter++;
            return Task.FromResult(new TestCacheItem { Value = cacheValue });
        }

        var item1 = await _typedCache.GetAsync(cacheKey, GetCacheValue);

        ChangeHttpContext();
        var item2 = await _typedCache.GetAsync(cacheKey, GetCacheValue);

        await RedisDatabase.Received(2).StringGetAsync(Arg.Any<RedisKey>());

        var cachedObject =
            RedisSerializer.Serialize(new TestCacheItem { Value = cacheValue }, typeof(TestCacheItem));
        await RedisDatabase.Received(2).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>());

        (await _typedCache.GetOrDefaultAsync(cacheKey)).Value.ShouldBe(cacheValue);

        counter.ShouldBe(2);

        item1.ShouldNotBe(null);
        item1.Value.ShouldBe(cacheValue);

        item2.ShouldNotBe(null);
        item2.Value.ShouldBe(cacheValue);
    }

    [Fact]
    public async Task Should_Not_Request_Again_For_Same_Context_Async()
    {
        var context1 = GetNewContextSubstitute();
        var context2 = GetNewContextSubstitute();

        string cacheKey = Guid.NewGuid().ToString();
        int counter = 0;
        int cacheValue = 1;

        Task<TestCacheItem> GetCacheValue()
        {
            counter++;
            return Task.FromResult(new TestCacheItem { Value = cacheValue });
        }

        CurrentHttpContext = context1;
        var item1 = await _typedCache.GetAsync(cacheKey, GetCacheValue); //First request

        CurrentHttpContext = context2;
        var item2 = await _typedCache.GetAsync(cacheKey, GetCacheValue); //Second request

        CurrentHttpContext = context1;
        var item3 = await _typedCache.GetAsync(cacheKey, GetCacheValue); //First request again

        CurrentHttpContext = context2;
        var item4 = await _typedCache.GetAsync(cacheKey, GetCacheValue); //Second request again

        await RedisDatabase.Received(2).StringGetAsync(Arg.Any<RedisKey>());

        var cachedObject =
            RedisSerializer.Serialize(new TestCacheItem { Value = cacheValue }, typeof(TestCacheItem));

        await RedisDatabase.Received(2).StringSetAsync(Arg.Any<RedisKey>(), cachedObject, Arg.Any<TimeSpan>());

        (await _typedCache.GetOrDefaultAsync(cacheKey)).Value.ShouldBe(cacheValue);

        counter.ShouldBe(2);
        item1.ShouldNotBe(null);
        item1.Value.ShouldBe(cacheValue);

        item2.ShouldNotBe(null);
        item2.Value.ShouldBe(cacheValue);

        item3.ShouldNotBe(null);
        item3.Value.ShouldBe(cacheValue);

        item4.ShouldNotBe(null);
        item4.Value.ShouldBe(cacheValue);
    }

    #endregion

    [Serializable]
    public class TestCacheItem
    {
        public int Value { get; set; }
    }
}