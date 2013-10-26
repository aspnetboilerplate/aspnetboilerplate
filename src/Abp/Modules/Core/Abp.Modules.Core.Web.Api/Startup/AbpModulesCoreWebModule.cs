using Abp.Modules.Core.Application.Services;
using Abp.Modules.Core.Dependency.Installers;
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

            BuildApiController
                .For<IUserService>("abp/user")
                .Build();
        }
    }
}
