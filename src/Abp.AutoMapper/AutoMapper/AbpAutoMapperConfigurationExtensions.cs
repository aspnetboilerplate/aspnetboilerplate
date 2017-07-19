using Abp.Configuration.Startup;

namespace Abp.AutoMapper
{
    /// <summary>
    /// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure Abp.AutoMapper module.
    /// </summary>
    public static class AbpAutoMapperConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.AutoMapper module.
        /// </summary>
        public static IAbpAutoMapperConfiguration AbpAutoMapper(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpAutoMapperConfiguration>();
        }
    }
}