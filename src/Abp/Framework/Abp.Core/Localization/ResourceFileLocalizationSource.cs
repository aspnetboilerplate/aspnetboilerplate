using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

namespace Abp.Localization 
{
    public class ResourceFileLocalizationSource : ILocalizationSource
    {
        public ResourceManager ResourceManager { get; private set; }

        public string SourceName { get; private set; }

        public ResourceFileLocalizationSource(string sourceName, ResourceManager resourceManager)
        {
            SourceName = sourceName;
            ResourceManager = resourceManager;
        }

        public virtual string GetString(string name)
        {
            return ResourceManager.GetString(name);
        }

        public virtual string GetString(string name, CultureInfo culture)
        {
            return ResourceManager.GetString(name, culture);
        } 

        public virtual IList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture);
        }

        public virtual IList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            
            return ResourceManager
                .GetResourceSet(culture, true, true)
                .Cast<DictionaryEntry>()
                .Select(entry => new LocalizedString(entry.Key.ToString(), entry.Value.ToString()))
                .ToList();
        }
    }
}
