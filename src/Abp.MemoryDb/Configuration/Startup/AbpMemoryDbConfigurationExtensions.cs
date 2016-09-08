using Abp.MemoryDb.Configuration;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP MemoryDb module.
    /// </summary>
    public static class AbpMemoryDbConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP MemoryDb module.
        /// </summary>
        public static IAbpMemoryDbModuleConfiguration AbpMemoryDb(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpMemoryDbModuleConfiguration>();
        }
    }
}