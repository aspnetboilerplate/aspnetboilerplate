namespace Abp.Modules
{
    public interface IAbpModuleManager
    {
        void InitializeModules();

        void ShutdownModules();
    }
}