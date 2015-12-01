using Abp.Modules;
using System.Reflection;

namespace Abp.RedisCache
{
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpRedisCacheModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
