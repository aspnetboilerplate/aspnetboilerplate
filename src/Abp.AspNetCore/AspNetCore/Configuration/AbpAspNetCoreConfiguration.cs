using System.Collections.Generic;
using System.Reflection;

namespace Abp.AspNetCore.Configuration
{
    public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
    {
        public List<AbpServiceControllerSetting> ServiceControllerSettings { get; }

        public AbpAspNetCoreConfiguration()
        {
            ServiceControllerSettings = new List<AbpServiceControllerSetting>();
        }

        public void CreateControllersForAppServices(Assembly assembly, string moduleName = AbpServiceControllerSetting.DefaultServiceModuleName)
        {
            ServiceControllerSettings.Add(new AbpServiceControllerSetting(moduleName, assembly));
        }
    }
}