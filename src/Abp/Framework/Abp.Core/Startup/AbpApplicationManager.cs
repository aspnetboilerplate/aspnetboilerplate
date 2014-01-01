using Abp.Modules;

namespace Abp.Startup
{
    public class AbpApplicationManager
    {
        private readonly AbpModuleManager _moduleManager;

        public AbpApplicationManager(AbpModuleManager moduleManager)
        {
            _moduleManager = moduleManager;
        }

        public virtual void Initialize()
        {
            var initializationContext = new AbpInitializationContext(_moduleManager.Modules);
            _moduleManager.Initialize(initializationContext); //TODO: _moduleManager.Modules!!!
        }

        public virtual void Dispose()
        {
            _moduleManager.Shutdown();
        }

    }
}
