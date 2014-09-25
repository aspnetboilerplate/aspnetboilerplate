using System.Resources;
using Abp.Localization.Sources.Resource;
using Abp.Localization.Sources.Xml;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// Extension methods to configure localization system.
    /// </summary>
    public static class AbpLocalizationConfigurationExtensions
    {
        /// <summary>
        /// Registers an XML based localization source.
        /// </summary>
        /// <param name="localizationConfiguration">Localization configuration object</param>
        /// <param name="sourceName">Unique Name of the source</param>
        /// <param name="directoryPath">Directory path of the localization XML files</param>
        public static void RegisterXmlSource(this AbpLocalizationConfiguration localizationConfiguration, string sourceName, string directoryPath)
        {
            localizationConfiguration.RegisterSource(new XmlLocalizationSource(sourceName, directoryPath));
        }

        /// <summary>
        /// Registers a resource file based localization source.
        /// </summary>
        /// <param name="localizationConfiguration">Localization configuration object</param>
        /// <param name="sourceName">Unique Name of the source</param>
        /// <param name="resourceManager">Directory path of the localization XML files</param>
        public static void RegisterResourceFileSource(this AbpLocalizationConfiguration localizationConfiguration, string sourceName, ResourceManager resourceManager)
        {
            localizationConfiguration.RegisterSource(new ResourceFileLocalizationSource(sourceName, resourceManager));
        }
    }
}