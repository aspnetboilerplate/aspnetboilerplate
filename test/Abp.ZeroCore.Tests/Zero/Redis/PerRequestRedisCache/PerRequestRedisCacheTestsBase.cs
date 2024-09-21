using System.Collections.Generic;
using Abp.Modules;
using Abp.Runtime.Caching.Redis;
using Abp.TestBase;
using Castle.MicroKernel.Registration;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using StackExchange.Redis;

namespace Abp.Zero.Redis.PerRequestRedisCache;

public abstract class PerRequestRedisCacheTestsBase<TStartupModule> : AbpIntegratedTestBase<TStartupModule>
where TStartupModule : AbpModule
{
    protected IDatabase RedisDatabase;
    protected IRedisCacheSerializer RedisSerializer;
    protected HttpContext CurrentHttpContext;

    protected override void PreInitialize()
    {
        CurrentHttpContext = GetNewContextSubstitute();

        RedisDatabase = Substitute.For<IDatabase>();

        var redisDatabaseProvider = Substitute.For<IAbpRedisCacheDatabaseProvider>();
        redisDatabaseProvider.GetDatabase().Returns(RedisDatabase);

        LocalIocManager.IocContainer.Register(Component.For<IAbpRedisCacheDatabaseProvider>().Instance(redisDatabaseProvider).LifestyleSingleton().IsDefault());
    }

    protected PerRequestRedisCacheTestsBase()
    {
        var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
        httpContextAccessor.HttpContext.Returns(info => CurrentHttpContext);

        LocalIocManager.IocContainer.Register(Component.For<IHttpContextAccessor>().Instance(httpContextAccessor).LifestyleSingleton().IsDefault());

        RedisSerializer = LocalIocManager.Resolve<IRedisCacheSerializer>();
    }

    protected HttpContext GetNewContextSubstitute()
    {
        var httpContext = Substitute.For<HttpContext>();
        httpContext.Items = new Dictionary<object, object>();
        return httpContext;
    }

    protected void ChangeHttpContext()
    {
        CurrentHttpContext = GetNewContextSubstitute();
    }
}