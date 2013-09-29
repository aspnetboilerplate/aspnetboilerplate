using Abp.Modules.Core.Dependency.Installers;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Web.Api", Dependencies = new[] { "Abp.Web.Api" })]
    public class AbpModulesCoreWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleWebApiInstaller());
        }
    }
}
