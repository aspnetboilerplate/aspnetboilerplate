namespace Adorable.Modules
{
    internal interface IAbpModuleManager
    {
        void InitializeModules();

        void ShutdownModules();
    }
}