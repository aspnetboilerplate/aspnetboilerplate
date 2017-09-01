using Abp.Configuration.Startup;

namespace Abp.Zero.Ldap.Configuration
{
    /// <summary>
    /// Extension methods for module zero configurations.
    /// </summary>
    public static class ModuleZeroLdapConfigurationExtensions
    {
        /// <summary>
        /// Configures ABP Zero LDAP module.
        /// </summary>
        /// <returns></returns>
        public static IAbpZeroLdapModuleConfig ZeroLdap(this IModuleConfigurations moduleConfigurations)
        {
            return moduleConfigurations.AbpConfiguration.Get<IAbpZeroLdapModuleConfig>();
        }
    }
}
