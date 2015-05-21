using Abp.RavenDb.Configuration;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP RavenDb module.
    /// </summary>
    public static class AbpRavenDbConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP RavenDb module.
        /// </summary>
        public static IAbpRavenDbModuleConfiguration AbpRavenDb(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.RavenDb", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpRavenDbModuleConfiguration>());
        }
    }
}