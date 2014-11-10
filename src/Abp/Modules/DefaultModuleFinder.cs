using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Reflection;
using Castle.Core.Logging;

namespace Abp.Modules
{
    internal class DefaultModuleFinder : IModuleFinder
    {
        public ILogger Logger { get; set; }

        public IAssemblyFinder AssemblyFinder { get; set; }

        public DefaultModuleFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
            Logger = NullLogger.Instance;
        }

        public List<Type> FindAll()
        {
            var allModules = new List<Type>();

            var allAssemblies = AssemblyFinder.GetAllAssemblies().Distinct();

            foreach (var assembly in allAssemblies)
            {
                Logger.Debug("Searching ABP modules in assembly: " + assembly.FullName);

                Type[] types;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                if (types.IsNullOrEmpty())
                {
                    continue;
                }

                var modules = (from type in types where AbpModule.IsAbpModule(type) select type).ToArray();

                if (modules.IsNullOrEmpty())
                {
                    continue;
                }

                Logger.Debug("Found modules:");
                foreach (var module in modules)
                {
                    Logger.Debug("- " + module.FullName);
                }

                allModules.AddRange(modules);
            }

            Logger.Debug("Found " + allModules.Count + " ABP modules in total.");

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