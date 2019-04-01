using System.Reflection;
using Abp.Modules;
using Abp.RealTime.Redis;

namespace Abp.OnlineClientStore.Redis.Tests
{
    [DependsOn(typeof(AbpRedisOnlineClientStoreModule))]
    public class AbpRedisOnlineClientStoreTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.AbpRedisOnlineClientStore().DatabaseId = 2;
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
