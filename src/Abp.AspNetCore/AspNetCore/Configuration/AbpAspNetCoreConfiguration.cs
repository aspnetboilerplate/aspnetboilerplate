using System.Collections.Generic;
using System.Reflection;
using Abp.Web.Models;

namespace Abp.AspNetCore.Configuration
{
    public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; set; }

        public List<AbpServiceControllerSetting> ServiceControllerSettings { get; }

        public AbpAspNetCoreConfiguration()
        {
            DefaultWrapResultAttribute = new WrapResultAttribute();
            ServiceControllerSettings = new List<AbpServiceControllerSetting>();
        }

        public void CreateControllersForAppServices(Assembly assembly, string moduleName = AbpServiceControllerSetting.DefaultServiceModuleName, bool useConventionalHttpVerbs = false)
        {
            ServiceControllerSettings.Add(new AbpServiceControllerSetting(moduleName, assembly, useConventionalHttpVerbs));
        }
    }
}