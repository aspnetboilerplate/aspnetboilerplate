using System;
using System.Collections.Generic;
using System.Linq;

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

        public List<Type> FindAll()
        {
            var allAssemblies = AssemblyFinder.GetAllAssemblies();

            return (
                from assembly in allAssemblies
                from type in assembly.GetTypes()
                where AbpModuleHelper.IsAbpModule(type)
                select type
                ).ToList();
        }
    }
}