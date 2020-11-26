using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;

namespace Abp.Zero.Redis
{
    [DependsOn(typeof(AbpAspNetCorePerRequestRedisCacheModule))]
    public class AbpPerRequestRedisCacheTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Caching.UseRedis();
        }
        
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpPerRequestRedisCacheTestModule).GetAssembly());
        }
    }
}