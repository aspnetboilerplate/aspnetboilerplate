using Abp.Configuration.Startup;

namespace Abp.AspNetCore.Configuration
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP ASP.NET Core module.
    /// </summary>
    public static class AbpAspNetCoreConfigurationExtensions
    {
        /// <summary>
        /// Used to configure ABP ASP.NET Core module.
        /// </summary>
        public static IAbpAspNetCoreConfiguration AbpAspNetCore(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpAspNetCoreConfiguration>();
        }
    }
}