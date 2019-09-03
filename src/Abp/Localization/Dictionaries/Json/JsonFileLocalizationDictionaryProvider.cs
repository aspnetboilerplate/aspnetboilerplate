using System.IO;
using Abp.Localization.Sources;

namespace Abp.Localization.Dictionaries.Json
{
    /// <summary>
    ///     Provides localization dictionaries from json files in a directory.
    /// </summary>
    public class JsonFileLocalizationDictionaryProvider : LocalizationDictionaryProviderBase
    {
        private readonly string _directoryPath;

        /// <summary>
        ///     Creates a new <see cref="JsonFileLocalizationDictionaryProvider" />.
        /// </summary>
        /// <param name="directoryPath">Path of the dictionary that contains all related XML files</param>
        public JsonFileLocalizationDictionaryProvider(string directoryPath)
        {
            _directoryPath = directoryPath;
        }

        protected override void InitializeDictionaries()
        {
            var fileNames = Directory.GetFiles(_directoryPath, "*.json", SearchOption.TopDirectoryOnly);

            foreach (var fileName in fileNames)
            {
                InitializeDictionary(CreateJsonLocalizationDictionary(fileName), isDefault: fileName.EndsWith(SourceName + ".json"));
            }
        }

        protected virtual JsonLocalizationDictionary CreateJsonLocalizationDictionary(string fileName)
        {
            return JsonLocalizationDictionary.BuildFromFile(fileName);
        }
    }
}
