using System.Linq;
using Abp.Dependency;
using Abp.Runtime.Caching;
using Abp.Runtime.Caching.Memory;
using AutoMapper.Internal;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Shouldly;
using Xunit;

namespace AbpAspNetCoreDemo.IntegrationTests.Tests;

public class MemoryCacheOptions_Test : IClassFixture<WebApplicationFactory<Startup>>
{
    private readonly WebApplicationFactory<Startup> _applicationFactory;

    public MemoryCacheOptions_Test()
    {
        _applicationFactory = new WebApplicationFactory<Startup>();
        _applicationFactory.CreateClient();
    }

    [Fact]
    public void MemoryCacheOption_Size_Test()
    {
        var memoryCacheManager = _applicationFactory.Services.GetService(typeof(ICacheManager)) as ICacheManager;

        memoryCacheManager.ShouldNotBeNull();
        memoryCacheManager.GetType().ShouldBe(typeof(AbpMemoryCacheManager));

        var memoryCache = memoryCacheManager.GetCache("Test");

        memoryCache.ShouldNotBeNull();
        memoryCache.GetType().ShouldBe(typeof(AbpMemoryCache));

        var memberPath = ReflectionHelper.GetMemberPath(typeof(AbpMemoryCache), "_memoryCacheOptions").First();
        var memoryCacheOptions = memberPath.GetMemberValue(memoryCache) as IOptions<MemoryCacheOptions>;
        memoryCacheOptions.ShouldNotBeNull();
        memoryCacheOptions.Value.SizeLimit.ShouldBe(2048);
    }

    [Fact]
    public void MemoryCacheOption_SizeLimit_Test()
    {
        new AbpMemoryCache("test", new MemoryCacheOptions
        {
            SizeLimit = 256,
        }).Set("test", "test");
    }
}