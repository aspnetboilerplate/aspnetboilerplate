using Abp.Configuration.Startup;

namespace Abp.Hangfire.Configuration
{
    public static class AbpHangfireConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP NHibernate module.
        /// </summary>
        public static IAbpHangfireConfiguration AbpNHibernate(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.GetOrCreate("Modules.Abp.Hangfire", () => configurations.AbpConfiguration.IocManager.Resolve<IAbpHangfireConfiguration>());
        }
    }
}