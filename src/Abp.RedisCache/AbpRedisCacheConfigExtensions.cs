using Adorable.Configuration.Startup;

namespace Adorable
{
    public static class AbpRedisCacheConfigExtensions
    {
        public static AbpRedisCacheConfig AbpRedisCacheModule(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration
                .GetOrCreate("AbpRedisCacheModule",
                    () => moduleConfigurations.AbpConfiguration.IocManager.Resolve<AbpRedisCacheConfig>()
                );
        }
    }
}