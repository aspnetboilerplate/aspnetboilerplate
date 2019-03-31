using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace Abp.RealTime.Redis
{
    /// <summary>
    /// This modules is used to replace ABP's in-memory online client store system with Redis server.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]
    public class AbpRedisOnlineClientStoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.Register<IAbpRedisOnlineClientStoreOptions, AbpRedisOnlineClientStoreOptions>();
            Configuration.ReplaceService<IOnlineClientStore, AbpRedisOnlineClientStore>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpRedisOnlineClientStoreModule).GetAssembly());
        }
    }
}
