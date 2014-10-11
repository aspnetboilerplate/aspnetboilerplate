using Abp.Configuration.Startup;

namespace Abp.NHibernate.Config
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP NHibernate module.
    /// </summary>
    public static class AbpNHibernateConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP NHibernate module.
        /// </summary>
        public static IAbpNHibernateModuleConfiguration AbpNHibernate(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.NHibernate", () => new AbpNHibernateModuleConfiguration());
        }
    }
}