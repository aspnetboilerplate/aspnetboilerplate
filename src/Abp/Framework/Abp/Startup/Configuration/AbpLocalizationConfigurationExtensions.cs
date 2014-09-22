using System.Resources;
using Abp.Localization.Sources.Resource;
using Abp.Localization.Sources.Xml;

namespace Abp.Startup.Configuration
{
    public static class AbpLocalizationConfigurationExtensions
    {
        public static void AddXmlSource(this AbpLocalizationConfiguration localizationConfiguration, string sourceName, string directory)
        {
            localizationConfiguration.AddSource(new XmlLocalizationSource(sourceName, directory));
        }
        public static void AddResourceFileSource(this AbpLocalizationConfiguration localizationConfiguration, string sourceName, ResourceManager resourceManager)
        {
            localizationConfiguration.AddSource(new ResourceFileLocalizationSource(sourceName, resourceManager));
        }
    }
}