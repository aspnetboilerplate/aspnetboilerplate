using System.Collections.Generic;
using System.Reflection;
using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.AspNetCore.Configuration
{
    public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; }

        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        public bool IsValidationEnabledForControllers { get; set; }

        public List<AbpServiceControllerSetting> ServiceControllerSettings { get; }

        public AbpAspNetCoreConfiguration()
        {
            DefaultWrapResultAttribute = new WrapResultAttribute();
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            ServiceControllerSettings = new List<AbpServiceControllerSetting>();
            IsValidationEnabledForControllers = true;
        }

        public void CreateControllersForAppServices(Assembly assembly, string moduleName = AbpServiceControllerSetting.DefaultServiceModuleName, bool useConventionalHttpVerbs = true)
        {
            ServiceControllerSettings.Add(new AbpServiceControllerSetting(moduleName, assembly, useConventionalHttpVerbs));
        }
    }
}