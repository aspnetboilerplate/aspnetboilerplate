using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Abp.Localization.Dictionaries.Xml
{
    /// <summary>
    /// Provides localization dictionaries from XML files embedded into an <see cref="Assembly"/>.
    /// </summary>
    public class XmlEmbeddedFileLocalizationDictionaryProvider : LocalizationDictionaryProviderBase
    {
        private readonly Assembly _assembly;
        private readonly string _rootNamespace;

        /// <summary>
        /// Creates a new <see cref="XmlEmbeddedFileLocalizationDictionaryProvider"/> object.
        /// </summary>
        /// <param name="assembly">Assembly that contains embedded xml files</param>
        /// <param name="rootNamespace">Namespace of the embedded xml dictionary files</param>
        public XmlEmbeddedFileLocalizationDictionaryProvider(Assembly assembly, string rootNamespace)
        {
            _assembly = assembly;
            _rootNamespace = rootNamespace;
        }

        protected override void InitializeDictionaries()
        {
            var allCultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures);
            var resourceNames = _assembly.GetManifestResourceNames().Where(resouceName =>
                allCultureInfos.Any(culture => resouceName.EndsWith($"{SourceName}.xml", true, null) ||
                                               resouceName.EndsWith($"{SourceName}-{culture.Name}.xml", true,
                                                   null))).ToList();
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith(_rootNamespace))
                {
                    using (var stream = _assembly.GetManifestResourceStream(resourceName))
                    {
                        var xmlString = Utf8Helper.ReadStringFromStream(stream);
                        InitializeDictionary(CreateXmlLocalizationDictionary(xmlString), isDefault: resourceName.EndsWith(SourceName + ".xml"));
                    }
                }
            }
        }

        protected virtual XmlLocalizationDictionary CreateXmlLocalizationDictionary(string xmlString)
        {
            return XmlLocalizationDictionary.BuildFomXmlString(xmlString);
        }
    }
}
