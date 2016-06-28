using System.Reflection;

namespace Abp.AspNetCore.Configuration
{
    public interface IAbpAspNetCoreConfiguration
    {
        void CreateControllersForAppServices(Assembly assembly, string moduleName = AbpServiceControllerSetting.DefaultServiceModuleName);
    }
}
