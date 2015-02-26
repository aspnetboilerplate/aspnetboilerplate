using System.IO;
using System.Linq;
using Abp.Localization.Dictionaries.Xml;

namespace Abp.Localization.Sources.Xml
{
    /// <summary>
    /// Provides localization dictionaries from files in a directory.
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
        
        public void AddDictionariesToLocalizationSource(IDictionaryBasedLocalizationSource localizationSource)
        {
            var files = Directory.GetFiles(_directoryPath, "*.xml", SearchOption.TopDirectoryOnly);
            var defaultLangFile = files.FirstOrDefault(f => f.EndsWith(localizationSource.Name + ".xml"));
            if (defaultLangFile == null)
            {
                throw new AbpException("Can not find default localization file for source " + localizationSource.Name + ". A source must contain a source-name.xml file as default localization.");
            }

            localizationSource.AddDictionary(XmlLocalizationDictionary.BuildFomFile(defaultLangFile), true);
            foreach (var file in files.Where(f => f != defaultLangFile))
            {
                localizationSource.AddDictionary(XmlLocalizationDictionary.BuildFomFile(file));
            }
        }
    }
}