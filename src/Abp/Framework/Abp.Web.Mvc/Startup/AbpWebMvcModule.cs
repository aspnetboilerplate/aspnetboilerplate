using System;
using System.Reflection;
using System.Web.Mvc;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup.Application;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.Resources.Embedded;
using Abp.Web.Startup;

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

        public override void PreInitialize()
        {
            base.PreInitialize();
            IocManager.AddConventionalRegisterer(new ControllerConventionalRegisterer());
        }

        public override void Initialize()
        {
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager.IocContainer.Kernel));
            GlobalFilters.Filters.Add(new AbpHandleErrorAttribute());
        }
    }
}
