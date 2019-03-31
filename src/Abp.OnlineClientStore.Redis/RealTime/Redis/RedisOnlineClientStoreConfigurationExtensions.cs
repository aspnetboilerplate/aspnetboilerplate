using Abp.Configuration.Startup;

namespace Abp.RealTime.Redis
{
    public static class RedisOnlineClientStoreConfigurationExtensions
    {
        public static IAbpRedisOnlineClientStoreOptions AbpRedisOnlineClientStore(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpRedisOnlineClientStoreOptions>();
        }
    }
}
