using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.Runtime.Caching.Redis;

namespace Abp.Zero.Redis.PerRequestRedisCache;

[DependsOn(typeof(AbpAspNetCorePerRequestRedisCacheModule))]
public class AbpPerRequestRedisCacheReplacementModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Caching.UseRedis(usePerRequestRedisCache: true);
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(
            typeof(AbpPerRequestRedisCacheReplacementModule).GetAssembly());
    }
}