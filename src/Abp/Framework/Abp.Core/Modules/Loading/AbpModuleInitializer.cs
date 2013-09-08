using System.Collections.Generic;

namespace Abp.Modules.Loading
{
    internal class AbpModuleInitializer
    {
        private readonly List<AbpModuleInfo> _modules;
        private readonly IAbpInitializationContext _initializationContext;

        public AbpModuleInitializer(List<AbpModuleInfo> modules, IAbpInitializationContext initializationContext)
        {
            _modules = modules;
            _initializationContext = initializationContext;
        }

        public void PreInitializeModules()
        {
            foreach (var module in _modules)
            {
                module.Instance.PreInitialize(_initializationContext);
            }
        }

        public void InitializeModules()
        {
            foreach (var module in _modules)
            {
                module.Instance.Initialize(_initializationContext);
            }
        }

        public void PostInitializeModules()
        {
            foreach (var module in _modules)
            {
                module.Instance.PostInitialize(_initializationContext);
            }
        }
    }
}
