using System.Collections.Generic;
using System.IO;
using Abp.Localization.Dictionaries.Xml;

namespace Abp.Localization.Sources.Xml
{
    /// <summary>
    /// Provides localization dictionaries from XML files in a directory.
    /// </summary>
    public class XmlFileLocalizationDictionaryProvider : ILocalizationDictionaryProvider
    {
        private readonly string _directoryPath;

        /// <summary>
        /// Creates a new <see cref="XmlFileLocalizationDictionaryProvider"/>.
        /// </summary>
        /// <param name="directoryPath">Path of the dictionary that contains all related XML files</param>
        public XmlFileLocalizationDictionaryProvider(string directoryPath)
        {
            if (!Path.IsPathRooted(directoryPath))
            {
                directoryPath = Path.Combine(XmlLocalizationSource.RootDirectoryOfApplication, directoryPath);
            }

            _directoryPath = directoryPath;
        }

        public IEnumerable<LocalizationDictionaryInfo> GetDictionaries(string sourceName)
        {
            var fileNames = Directory.GetFiles(_directoryPath, "*.xml", SearchOption.TopDirectoryOnly);

            var dictionaries = new List<LocalizationDictionaryInfo>();

            foreach (var fileName in fileNames)
            {
                dictionaries.Add(
                    new LocalizationDictionaryInfo(
                        XmlLocalizationDictionary.BuildFomFile(fileName),
                        isDefault: fileName.EndsWith(sourceName + ".xml")
                        )
                    );
            }

            return dictionaries;
        }
    }
}