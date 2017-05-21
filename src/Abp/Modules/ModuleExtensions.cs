using System.Linq;

using Abp.Configuration.Startup;

namespace Abp.Modules
{
    public static class ModuleExtensions
    {
        public static bool IsADependedModule<TModule>(this IModuleConfigurations modules) where TModule : AbpModule
        {
            var moduleManager = modules.AbpConfiguration.IocManager.Resolve<IAbpModuleManager>();
            return moduleManager.Modules.Any(x => x.Type == typeof(TModule));
        }
    }
}
