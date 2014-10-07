using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Reflection;

namespace Abp.Modules
{
    internal class DefaultModuleFinder : IModuleFinder
    {
        public IAssemblyFinder AssemblyFinder { get; set; }

        public DefaultModuleFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
        }

        public List<Type> FindAll()
        {
            var allModules = new List<Type>();            
            
            allModules.AddRange(
                from assembly in AssemblyFinder.GetAllAssemblies()
                from type in assembly.GetTypes()
                where AbpModule.IsAbpModule(type)
                select type
                );

            var currentModules = allModules.ToList();

            foreach (var module in currentModules)
            {
                FillDependedModules(module, allModules);
            }
            
            return allModules;
        }

        private void FillDependedModules(Type module, List<Type> allModules)
        {
            foreach (var dependedModule in AbpModule.FindDependedModuleTypes(module))
            {
                if (!allModules.Contains(dependedModule))
                {
                    allModules.Add(dependedModule);
                    FillDependedModules(dependedModule, allModules);
                }
            }
        }
    }
}