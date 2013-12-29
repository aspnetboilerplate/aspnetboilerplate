using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Threading;

namespace Abp.Localization 
{
    /// <summary>
    /// This class is used to simplify to create a localization source that
    /// uses resource files.
    /// </summary>
    public class ResourceFileLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Reference to the <see cref="ResourceManager"/> object related to this localization source.
        /// </summary>
        public ResourceManager ResourceManager { get; private set; }

        /// <param name="name">Unique Name of the source</param>
        /// <param name="resourceManager">Reference to the <see cref="ResourceManager"/> object related to this localization source</param>
        public ResourceFileLocalizationSource(string name, ResourceManager resourceManager)
        {
            Name = name;
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

        public virtual IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture);
        }

        public virtual IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            return ResourceManager
                .GetResourceSet(culture, true, true) //TODO: true or false for createIfNotExists? Test it's effect.
                .Cast<DictionaryEntry>()
                .Select(entry => new LocalizedString(entry.Key.ToString(), entry.Value.ToString()))
                .ToImmutableList();
        }
    }
}
