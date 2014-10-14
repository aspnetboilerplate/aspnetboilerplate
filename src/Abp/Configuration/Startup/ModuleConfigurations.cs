namespace Abp.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IAbpStartupConfiguration AbpConfiguration { get; private set; }

        public ModuleConfigurations(IAbpStartupConfiguration abpConfiguration)
        {
            AbpConfiguration = abpConfiguration;
        }
    }
}