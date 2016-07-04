using System.Reflection;
using Abp.Web.Models;

namespace Abp.AspNetCore.Configuration
{
    public interface IAbpAspNetCoreConfiguration
    {
        WrapResultAttribute DefaultWrapResultAttribute { get; set; }

        void CreateControllersForAppServices(Assembly assembly, string moduleName = AbpServiceControllerSetting.DefaultServiceModuleName, bool useConventionalHttpVerbs = false);
    }
}
