using System.Reflection;
using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.AspNetCore.Configuration
{
    public interface IAbpAspNetCoreConfiguration
    {
        WrapResultAttribute DefaultWrapResultAttribute { get; }

        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

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

        void CreateControllersForAppServices(Assembly assembly, string moduleName = AbpServiceControllerSetting.DefaultServiceModuleName, bool useConventionalHttpVerbs = true);
    }
}
