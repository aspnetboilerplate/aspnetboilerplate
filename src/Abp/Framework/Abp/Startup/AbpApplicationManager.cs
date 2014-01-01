using Abp.Modules;

namespace Abp.Startup
{
    public class AbpApplicationManager
    {
        private readonly AbpModuleManager _moduleManager;
        private readonly AbpModuleCollection _modules;

        public AbpApplicationManager(AbpModuleManager moduleManager, AbpModuleCollection modules)
        {
            _moduleManager = moduleManager;
            _modules = modules;
        }

        public virtual void Initialize()
        {
            var initializationContext = new AbpInitializationContext(_modules);
            _moduleManager.Initialize(initializationContext);
        }

        public virtual void Dispose()
        {
            _moduleManager.Shutdown();
        }
    }
}
