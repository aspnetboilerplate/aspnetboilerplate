namespace Abp.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public ModuleConfigurations(IAbpStartupConfiguration abpConfiguration)
        {
            AbpConfiguration = abpConfiguration;
        }

        public IAbpStartupConfiguration AbpConfiguration { get; }
    }
}