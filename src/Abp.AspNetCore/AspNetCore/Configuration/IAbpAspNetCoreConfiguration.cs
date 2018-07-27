using System;
using System.Collections.Generic;
using System.Reflection;
using Abp.AspNetCore.Mvc.Results.Caching;
using Abp.Domain.Uow;
using Abp.Web.Models;
using Microsoft.AspNetCore.Routing;

namespace Abp.AspNetCore.Configuration
{
    public interface IAbpAspNetCoreConfiguration
    {
        WrapResultAttribute DefaultWrapResultAttribute { get; }

        IClientCacheAttribute DefaultClientCacheAttribute { get; set; }

        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        List<Type> FormBodyBindingIgnoredTypes { get; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsValidationEnabledForControllers { get; set; }

        /// <summary>
        /// Used to enable/disable auditing for MVC controllers.
        /// Default: true.
        /// </summary>
        bool IsAuditingEnabled { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool SetNoCacheForAjaxResponses { get; set; }

        /// <summary>
        /// Default: false.
        /// </summary>
        bool UseMvcDateTimeFormatForAppServices { get; set; }

        /// <summary>
        /// Used to add route config for modules.
        /// </summary>
        List<Action<IRouteBuilder>> RouteConfiguration { get; }

        AbpControllerAssemblySettingBuilder CreateControllersForAppServices(
            Assembly assembly,
            string moduleName = AbpControllerAssemblySetting.DefaultServiceModuleName,
            bool useConventionalHttpVerbs = true
        );
    }
}
