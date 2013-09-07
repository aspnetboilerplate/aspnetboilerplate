using Abp.Startup;
using Abp.Web.Modules;
using Castle.Windsor.Installer;

namespace Abp.Modules.Core
{
    [AbpModule("Abp.Modules.Core")]
    public class AbpCoreModule : AbpModule
    {
        public override void Initialize(AbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            initializationContext.IocContainer.Install(FromAssembly.This());

            AutoMappingManager.Map();
        }
    }
}
