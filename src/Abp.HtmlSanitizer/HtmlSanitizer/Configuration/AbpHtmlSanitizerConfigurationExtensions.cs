using Abp.Configuration.Startup;

namespace Abp.HtmlSanitizer.Configuration;

/// <summary>
/// Defines extension methods to <see cref="IModuleConfigurations"/> to allow to configure ABP HTML sanitizer module.
/// </summary>
public static class AbpHtmlSanitizerConfigurationExtensions
{
    /// <summary>
    /// Used to configure ABP HTML sanitizer module.
    /// </summary>
    public static IHtmlSanitizerConfiguration AbpHtmlSanitizer(this IModuleConfigurations configurations)
    {
        return configurations.AbpConfiguration.Get<IHtmlSanitizerConfiguration>();
    }
}