using Abp.Modules.Core.Startup.Dependency.Installers;
using Abp.Startup;
using Abp.Users;
using Abp.WebApi.Controllers.Dynamic.Builders;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Web.Api", Dependencies = new[] { "Abp.Web.Api" })]
    public class AbpModulesCoreWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleWebApiInstaller());

            //TODO: Remove this for security reasons!
            DyanmicApiControllerBuilder
                .For<IUserAppService>("abp/user")
                .Build();
        }
    }
}
