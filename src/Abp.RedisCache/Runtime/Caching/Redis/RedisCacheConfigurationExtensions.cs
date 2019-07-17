using System;
using Abp.Dependency;
using Abp.RealTime;
using Abp.Runtime.Caching.Configuration;
using Abp.Runtime.Caching.Redis.OnlineClientStore;
using Abp.Configuration.Startup;

namespace Abp.Runtime.Caching.Redis
{
    /// <summary>
    /// Extension methods for <see cref="ICachingConfiguration"/>.
    /// </summary>
    public static class RedisCacheConfigurationExtensions
    {
        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration)
        {
            cachingConfiguration.UseRedis(options => { });
        }

        /// <summary>
        /// Configures caching to use Redis as cache server.
        /// </summary>
        /// <param name="cachingConfiguration">The caching configuration.</param>
        /// <param name="optionsAction">Ac action to get/set options</param>
        public static void UseRedis(this ICachingConfiguration cachingConfiguration, Action<AbpRedisCacheOptions> optionsAction)
        {
            var iocManager = cachingConfiguration.AbpConfiguration.IocManager;

            iocManager.RegisterIfNot<ICacheManager, AbpRedisCacheManager>();

            var options = iocManager.Resolve<AbpRedisCacheOptions>();
            optionsAction(options);

            if (!options.UseOnlineClientStoreWithRedisCache) return;

            if (iocManager.IsRegistered<IOnlineClientStore>())
            {
                cachingConfiguration.AbpConfiguration.ReplaceService<IOnlineClientStore, AbpRedisOnlineClientStore>();
            }
            else
            {
                iocManager.Register<IOnlineClientStore, AbpRedisOnlineClientStore>();
            }


        }
    }
}
