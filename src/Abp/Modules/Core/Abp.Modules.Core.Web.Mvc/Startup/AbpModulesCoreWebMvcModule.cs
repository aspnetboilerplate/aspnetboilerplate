using Abp.Modules.Core.Dependency.Installers;
using Abp.Modules.Core.Web.Authentication;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Web.Mvc", Dependencies = new[] { "Abp.Web.Mvc", "Abp.Modules.Core.Data" })]
    public class AbpModulesCoreWebMvcModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleWebMvcInstaller());
            AbpMembershipProvider.IocContainer = initializationContext.IocContainer;
        }
    }
}
