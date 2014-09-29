namespace Abp.Modules
{
    internal interface IAbpModuleManager
    {
        void InitializeModules();

        void ShutdownModules();
    }
}