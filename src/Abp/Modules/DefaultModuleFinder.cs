using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Dependency;
using Abp.Reflection;
using Castle.Core.Logging;
using Castle.MicroKernel.Registration;

namespace Abp.Modules
{
    internal class DefaultModuleFinder : IModuleFinder
    {
        public ILogger Logger { get; set; }

        public IAssemblyFinder AssemblyFinder { get; set; }

        public DefaultModuleFinder()
        {
            IocManager.Instance.IocContainer.Register(Component.For<IAssemblyFilter>().ImplementedBy<DefaultAssemblyFilter>());
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

                var modules = (from type in assembly.GetTypes()
                    where AbpModule.IsAbpModule(type)
                    select type).ToList();

                if (modules.Count > 0)
                {
                    Logger.Debug("Found modules:");
                    foreach (var module in modules)
                    {
                        Logger.Debug("- " + module.FullName);
                    }

                    allModules.AddRange(modules);
                }
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