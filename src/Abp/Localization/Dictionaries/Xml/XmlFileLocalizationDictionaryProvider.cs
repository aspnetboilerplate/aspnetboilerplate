using System.IO;

namespace Abp.Localization.Dictionaries.Xml
{
    /// <summary>
    /// Provides localization dictionaries from XML files in a directory.
    /// </summary>
    public class XmlFileLocalizationDictionaryProvider : LocalizationDictionaryProviderBase
    {
        private readonly string _directoryPath;

        /// <summary>
        /// Creates a new <see cref="XmlFileLocalizationDictionaryProvider"/>.
        /// </summary>
        /// <param name="directoryPath">Path of the dictionary that contains all related XML files</param>
        public XmlFileLocalizationDictionaryProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        protected override void InitializeDictionaries()
        {
            var fileNames = Directory.GetFiles(_directoryPath, "*.xml", SearchOption.TopDirectoryOnly);

            foreach (var fileName in fileNames)
            {
                InitializeDictionary(CreateXmlLocalizationDictionary(fileName), isDefault: fileName.EndsWith(SourceName + ".xml"));
            }
        }

        protected virtual XmlLocalizationDictionary CreateXmlLocalizationDictionary(string fileName)
        {
            return XmlLocalizationDictionary.BuildFomFile(fileName);
        }
    }
}
