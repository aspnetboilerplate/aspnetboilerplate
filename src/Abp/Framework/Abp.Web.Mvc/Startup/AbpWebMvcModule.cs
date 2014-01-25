using System.Reflection;
using System.Web.Mvc;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Dependency;
using Castle.Windsor.Installer;

namespace Abp.Web.Mvc.Startup
{
    /// <summary>
    /// This module is used to build ASP.NET MVC web sites using Abp.
    /// </summary>
    [AbpModule("Abp.Web.Mvc")]
    public class AbpWebMvcModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            IocManager.Instance.AddConventionalRegisterer(new MvcConventionalRegisterer());
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(initializationContext.IocContainer.Kernel));
            GlobalFilters.Filters.Add(new AbpHandleErrorAttribute());
        }
    }
}
