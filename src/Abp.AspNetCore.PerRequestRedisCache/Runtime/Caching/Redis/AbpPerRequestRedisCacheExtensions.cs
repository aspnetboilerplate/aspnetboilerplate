using System;
using Abp.Dependency;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Runtime.Caching.Redis;

public static class AbpPerRequestRedisCacheExtensions
{
    /// <summary>
    /// Configures caching to use Redis as cache server.
    /// </summary>
    /// <param name="cachingConfiguration">The caching configuration.</param>
    /// <param name="usePerRequestRedisCache">Replaces ICacheManager with <see cref="Abp.Runtime.Caching.Redis.AbpPerRequestRedisCacheManager"/></param>
    public static void UseRedis(this ICachingConfiguration cachingConfiguration, bool usePerRequestRedisCache)
    {
        cachingConfiguration.UseRedis(options => { }, usePerRequestRedisCache);
    }

    /// <summary>
    /// Configures caching to use Redis as cache server.
    /// </summary>
    /// <param name="cachingConfiguration">The caching configuration.</param>
    /// <param name="optionsAction">Action to get/set options</param>
    /// <param name="usePerRequestRedisCache">Replaces ICacheManager with <see cref="Abp.Runtime.Caching.Redis.AbpPerRequestRedisCacheManager"/></param>
    public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<AbpRedisCacheOptions> optionsAction, bool usePerRequestRedisCache)
    {
        if (!usePerRequestRedisCache)
        {
            cachingConfiguration.UseRedis(optionsAction);
            return;
        }

        var iocManager = cachingConfiguration.AbpConfiguration.IocManager;
        iocManager.RegisterIfNot<ICacheManager, AbpPerRequestRedisCacheManagerForReplacement>();

        optionsAction(iocManager.Resolve<AbpRedisCacheOptions>());
    }
}