using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultModuleFinder : IModuleFinder
    {
        public IAssemblyFinder AssemblyFinder { get; set; }

        public DefaultModuleFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;            
        }

        public List<AbpModuleInfo> FindAll()
        {
            var modules = new List<AbpModuleInfo>();

            var allAssemblies = AssemblyFinder.GetAllAssemblies();

            var scannedAssemlies = new List<Assembly>();
            foreach (var assembly in allAssemblies)
            {
                FillModules(modules, assembly, scannedAssemlies);
            }

            return modules;
        }

        private void FillModules(List<AbpModuleInfo> modules, Assembly assembly, List<Assembly> scannedAssemblies)
        {
            if (scannedAssemblies.Contains(assembly))
            {
                return;
            }

            scannedAssemblies.Add(assembly);

            foreach (var type in assembly.GetTypes())
            {
                //Skip types those are not Abp Module
                if (!AbpModuleHelper.IsAbpModule(type))
                {
                    continue;
                }

                //Prevent multiple adding same module
                var moduleInfo = modules.FirstOrDefault(m => m.Type == type);
                if (moduleInfo == null)
                {
                    moduleInfo = AbpModuleInfo.CreateForType(type);
                    modules.Add(moduleInfo);
                }

                //Check for depended modules
                var dependedModuleTypes = moduleInfo.Instance.GetDependedModules();
                foreach (var dependedModuleType in dependedModuleTypes)
                {
                    FillModules(modules, dependedModuleType.Assembly, scannedAssemblies);
                }
            }
        }
    }
}