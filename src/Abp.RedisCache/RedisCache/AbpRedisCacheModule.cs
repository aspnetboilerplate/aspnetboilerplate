using Abp.Modules;
using System.Reflection;

namespace Abp.RedisCache
{
    public class AbpRedisCacheModule : AbpModule
    {
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
