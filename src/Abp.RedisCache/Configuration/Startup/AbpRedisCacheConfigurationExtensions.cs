using Abp.Configuration.Startup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abp.RedisCache.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP MongoDb module.
    /// </summary>
    public static class AbpRedisCacheConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP Redis Cache module.
        /// </summary>
        public static IAbpRedisCacheModuleConfiguration RedisCache(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.RedisCache", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpRedisCacheModuleConfiguration>());
        }
    }
}
