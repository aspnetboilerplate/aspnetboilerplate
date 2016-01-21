using System.Collections.Generic;

namespace Abp.Localization.Dictionaries.Json
{
    /// <summary>
    /// Use it to serialize json file
    /// </summary>
    public class JsonLocalizationFile
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public JsonLocalizationFile()
        {
            Texts = new Dictionary<string, string>();
        }

        /// <summary>
        /// get or set the culture name; eg : en , en-us, zh-CN
        /// </summary>
        public string Culture { get; set; }

        /// <summary>
        ///  Key value pairs
        /// </summary>
        public Dictionary<string, string> Texts { get; private set; }
    }
}