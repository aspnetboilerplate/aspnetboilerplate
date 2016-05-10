using System;
using System.Reflection;
using System.Web.Mvc;
using Abp.Modules;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.ModelBinding.Binders;

namespace Abp.Web.Mvc
{
    /// <summary>
    ///     This module is used to build ASP.NET MVC web sites using Abp.
    /// </summary>
    [DependsOn(typeof(AbpWebModule))]
    public class AbpWebMvcModule : AbpModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
        }

        /// <inheritdoc />
        public override void PostInitialize()
        {
            var abpMvcDateTimeBinder = new AbpMvcDateTimeBinder();
            ModelBinders.Binders.Add(typeof(DateTime), abpMvcDateTimeBinder);
            ModelBinders.Binders.Add(typeof(DateTime?), abpMvcDateTimeBinder);
        }
    }
}