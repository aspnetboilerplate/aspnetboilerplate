using System.Collections.Generic;
using System.Reflection;

namespace Abp.AspNetCore.Configuration
{
    public class AbpAspNetCoreConfiguration : IAbpAspNetCoreConfiguration
    {
        public List<Assembly> ControllerAssemblies { get; }

        public AbpAspNetCoreConfiguration()
        {
            ControllerAssemblies = new List<Assembly>();
        }

        public void CreateControllersForAppServices(Assembly assembly)
        {
            ControllerAssemblies.Add(assembly);
        }
    }
}