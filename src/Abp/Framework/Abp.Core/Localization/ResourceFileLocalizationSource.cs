using System;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

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

        public virtual IList<string> GetAllStrings()
        {
            throw new NotImplementedException();
        }

        public virtual IList<string> GetAllStrings(CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
