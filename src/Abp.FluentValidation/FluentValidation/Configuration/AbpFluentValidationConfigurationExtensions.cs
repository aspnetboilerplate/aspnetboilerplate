using Abp.Configuration.Startup;

namespace Abp.FluentValidation.Configuration
{
    public static class AbpFluentValidationConfigurationExtensions
    {
        /// <summary>
        /// Used to configure Abp.FluentValidation module.
        /// </summary>
        public static IAbpFluentValidationConfiguration AbpFluentValidation(this IModuleConfigurations configurations)
        {
            return configurations.AbpConfiguration.Get<IAbpFluentValidationConfiguration>();
        }
    }
}
