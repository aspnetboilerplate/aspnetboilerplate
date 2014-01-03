using System.Collections.Generic;
using System.Globalization;

namespace Abp.Localization.Engine
{
    /// <summary>
    /// 
    /// </summary>
    internal class LocalizationDictionary : Dictionary<string, string>
    {
        public CultureInfo Culture { get; set; }

        public LocalizationDictionary()
        {

        }

        public LocalizationDictionary(CultureInfo culture)
        {
            Culture = culture;
        }
    }
}