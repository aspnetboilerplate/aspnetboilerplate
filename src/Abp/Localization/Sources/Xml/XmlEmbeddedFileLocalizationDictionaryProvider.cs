using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Abp.IO.Extensions;
using Abp.Localization.Dictionaries.Xml;

namespace Abp.Localization.Sources.Xml
{
    /// <summary>
    /// Provides localization dictionaries from XML files embedded into an <see cref="Assembly"/>.
    /// </summary>
    public class XmlEmbeddedFileLocalizationDictionaryProvider : ILocalizationDictionaryProvider
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

        public IEnumerable<LocalizationDictionaryInfo> GetDictionaries(string sourceName)
        {
            var dictionaries = new List<LocalizationDictionaryInfo>();

            var resourceNames = _assembly.GetManifestResourceNames();
            foreach (var resourceName in resourceNames)
            {
                if (resourceName.StartsWith(_rootNamespace)) //TODO: check sourceName
                {
                    using (var stream = _assembly.GetManifestResourceStream(resourceName))
                    {
                        var xmlString = Encoding.UTF8.GetString(stream.GetAllBytes());
                        dictionaries.Add(
                            new LocalizationDictionaryInfo(
                                XmlLocalizationDictionary.BuildFomXmlString(xmlString),
                                isDefault: resourceName.EndsWith(sourceName + ".xml")
                                )
                            );
                    }
                }
            }

            return dictionaries;
        }
    }
}