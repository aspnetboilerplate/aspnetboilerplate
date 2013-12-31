using Abp.Modules;

namespace Abp.Startup
{
    public class AbpApplicationManager
    {
        private readonly string _applicationDirectory;
        
        private readonly AbpModuleManager _moduleManager;

        public AbpApplicationManager(string applicationDirectory, AbpModuleManager moduleManager)
        {
            _applicationDirectory = applicationDirectory;
            _moduleManager = moduleManager;
        }

        public virtual void Initialize()
        {
            var initializationContext = new AbpInitializationContext(_moduleManager.Modules, _applicationDirectory);
            _moduleManager.InitializeModules(initializationContext); //TODO: _moduleManager.Modules!!!
        }

        public virtual void Dispose()
        {
            _moduleManager.ShutdownModules();
        }

    }
}
