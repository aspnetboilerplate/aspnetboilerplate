using System.Web.Mvc;
using Abp.Modules;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Dependency;
using Abp.Web.Mvc.Dependency.Installers;

namespace Abp.Web.Mvc.Startup
{
    [AbpModule("Abp.Web.Mvc")]
    public class AbpWebMvcModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(initializationContext.IocContainer.Kernel));
            initializationContext.IocContainer.Install(new AbpWebMvcInstaller());

            GlobalFilters.Filters.Add(new AbpHandleErrorAttribute());
        }
    }
}
