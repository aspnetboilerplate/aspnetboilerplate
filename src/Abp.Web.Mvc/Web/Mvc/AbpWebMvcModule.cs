using System;
using System.Collections.Generic;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Web.Mvc.Auditing;
using Abp.Web.Mvc.Authorization;
using Abp.Web.Mvc.Configuration;
using Abp.Web.Mvc.Controllers;
using Abp.Web.Mvc.ModelBinding.Binders;
using Abp.Web.Mvc.Security.AntiForgery;
using Abp.Web.Mvc.Uow;
using Abp.Web.Mvc.Validation;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Mvc
{
    /// <summary>
    /// This module is used to build ASP.NET MVC web sites using Abp.
    /// </summary>
    [DependsOn(typeof(AbpWebModule))]
    public class AbpWebMvcModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ControllerConventionalRegistrar());

            IocManager.Register<IAbpMvcConfiguration, AbpMvcConfiguration>();

            Configuration.ReplaceService<IAbpAntiForgeryManager, AbpMvcAntiForgeryManager>();

            AddIgnoredTypes();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            ControllerBuilder.Current.SetControllerFactory(new WindsorControllerFactory(IocManager));
        }

        /// <inheritdoc/>
        public override void PostInitialize()
        {
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcAuthorizeFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpAntiForgeryMvcFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcAuditFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcValidationFilter>());
            GlobalFilters.Filters.Add(IocManager.Resolve<AbpMvcUowFilter>());

            var abpMvcDateTimeBinder = new AbpMvcDateTimeBinder();
            ModelBinders.Binders.Add(typeof(DateTime), abpMvcDateTimeBinder);
            ModelBinders.Binders.Add(typeof(DateTime?), abpMvcDateTimeBinder);
        }

        private void AddIgnoredTypes()
        {
            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(HttpPostedFileBase));
            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(IEnumerable<HttpPostedFileBase>));
            Configuration.Auditing.IgnoredTypes.AddIfNotContains(typeof(HttpPostedFileBase));
            Configuration.Auditing.IgnoredTypes.AddIfNotContains(typeof(IEnumerable<HttpPostedFileBase>));
        }
    }
}
