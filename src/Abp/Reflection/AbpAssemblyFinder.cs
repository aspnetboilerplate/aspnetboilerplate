using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Modules;
using Abp.PlugIns;

namespace Abp.Reflection
{
    public class AbpAssemblyFinder : IAssemblyFinder
    {
        private readonly IAbpModuleManager _moduleManager;
        private readonly IAbpPlugInManager _plugInManager;

        public AbpAssemblyFinder(IAbpModuleManager moduleManager, IAbpPlugInManager plugInManager)
        {
            _moduleManager = moduleManager;
            _plugInManager = plugInManager;
        }

        public List<Assembly> GetAllAssemblies()
        {
            var assemblies = new List<Assembly>();

            foreach (var module in _moduleManager.Modules)
            {
                assemblies.Add(module.Assembly);
                assemblies.AddRange(module.Instance.GetAdditionalAssemblies());
            }

            assemblies.AddRange(_plugInManager.GetPlugInAssemblies());

            return assemblies.Distinct().ToList();
        }
    }
}