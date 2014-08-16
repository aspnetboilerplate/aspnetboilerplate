using System;
using System.Reflection;
using System.Web.Mvc;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Startup.Application;
using Abp.Startup.Web;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Resources.Embedded;

namespace Abp.Web.Mvc.Startup
{
    /// <summary>
    /// This module is used to build ASP.NET MVC web sites using Abp.
    /// </summary>
    public class AbpWebMvcModule : AbpModule
    {
        public override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof(AbpWebModule),
                       typeof(AbpApplicationModule),
                   };
        }

        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            IocManager.Instance.AddConventionalRegisterer(new ControllerConventionalRegisterer());
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
