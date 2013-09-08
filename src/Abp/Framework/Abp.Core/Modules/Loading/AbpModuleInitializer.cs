using System.Collections.Generic;

namespace Abp.Modules.Loading
{
    /// <summary>
    /// This class is used to initialize all modules.
    /// </summary>
    internal class AbpModuleInitializer
    {
        /// <summary>
        /// Reference to the list of all modules.
        /// </summary>
        private readonly List<AbpModuleInfo> _modules;

        /// <summary>
        /// Reference to the initialization context.
        /// </summary>
        private readonly IAbpInitializationContext _initializationContext;

        /// <summary>
        /// Creates a new AbpModuleInitializer object.
        /// </summary>
        /// <param name="modules">Reference to the list of all modules</param>
        /// <param name="initializationContext">Reference to the initialization context</param>
        public AbpModuleInitializer(List<AbpModuleInfo> modules, IAbpInitializationContext initializationContext)
        {
            _modules = modules;
            _initializationContext = initializationContext;
        }

        /// <summary>
        /// Calls <see cref="IAbpModule.PreInitialize"/> method of all modules.
        /// </summary>
        public void PreInitializeModules()
        {
            foreach (var module in _modules)
            {
                module.Instance.PreInitialize(_initializationContext);
            }
        }

        /// <summary>
        /// Calls <see cref="IAbpModule.Initialize"/> method of all modules.
        /// </summary>
        public void InitializeModules()
        {
            foreach (var module in _modules)
            {
                module.Instance.Initialize(_initializationContext);
            }
        }

        /// <summary>
        /// Calls <see cref="IAbpModule.PostInitialize"/> method of all modules.
        /// </summary>
        public void PostInitializeModules()
        {
            foreach (var module in _modules)
            {
                module.Instance.PostInitialize(_initializationContext);
            }
        }
    }
}
