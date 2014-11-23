using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Reflection;
using Castle.Core.Logging;

namespace Abp.Modules
{
    internal class DefaultModuleFinder : IModuleFinder
    {
        public ILogger Logger { get; set; }

        private readonly ITypeFinder _typeFinder;

        public DefaultModuleFinder(ITypeFinder typeFinder)
        {
            _typeFinder = typeFinder;
            Logger = NullLogger.Instance;
        }

        public List<Type> FindAll()
        {
            var modules = _typeFinder.Find(AbpModule.IsAbpModule).ToList();
            Logger.Debug("Found " + modules.Count + " ABP modules in total.");

            //Copying into new list since it will be modified by FillDependedModules
            var initialModuleList = modules.ToList(); 
            foreach (var module in initialModuleList)
            {
                FillDependedModules(module, modules);
            }

            return modules;
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