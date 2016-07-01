using System;
using System.Collections.Generic;

namespace Abp.Modules
{
    internal class DefaultModuleFinder : IModuleFinder
    {
        private readonly IAbpStartupModuleAccessor _startupModuleAccessor;

        private ICollection<Type> _modules;

        private readonly object _syncObj = new object();

        public DefaultModuleFinder(IAbpStartupModuleAccessor startupModuleAccessor)
        {
            _startupModuleAccessor = startupModuleAccessor;
        }

        public ICollection<Type> FindAll()
        {
            if (_modules == null)
            {
                lock (_syncObj)
                {
                    if (_modules == null)
                    {
                        _modules = AbpModule.FindDependedModuleTypesRecursively(_startupModuleAccessor.StartupModule);
                    }
                }
            }

            return _modules;
        }
    }
}