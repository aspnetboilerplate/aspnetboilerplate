using Abp.Configuration.Startup;

namespace Abp.Startup.Infrastructure.NHibernate
{
    /// <summary>
    /// Defines extension methods to <see cref="IAbpModuleConfigurations"/> to allow to configure ABP NHibernate module.
    /// </summary>
    public static class AbpNHibernateConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP NHibernate module.
        /// </summary>
        public static IAbpNHibernateModuleConfiguration AbpNHibernate(this IAbpModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.NHibernate", () => new AbpNHibernateModuleConfiguration());
        }
    }
}