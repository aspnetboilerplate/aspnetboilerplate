using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.Runtime.Caching.Redis
{
    [DependsOn(typeof(AbpRedisCacheModule))]
    public class AbpAspNetCorePerRequestRedisCacheModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpPerRequestRedisCache, AbpPerRequestRedisCache>();
            IocManager.Register<IAbpPerRequestRedisCacheManager, AbpPerRequestRedisCacheManager>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCorePerRequestRedisCacheModule).GetAssembly());
        }
    }
}