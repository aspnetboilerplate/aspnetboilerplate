using System.Collections.Generic;
using System.Globalization;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// Null object pattern for <see cref="ILocalizationSource"/>.
    /// </summary>
    internal class NullLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullLocalizationSource Instance { get { return SingletonInstance; } }
        private static readonly NullLocalizationSource SingletonInstance = new NullLocalizationSource();

        public string Name { get { return null; } }

        public void Initialize()
        {
            
        }

        public string GetString(string name)
        {
            return name;
        }

        public string GetString(string name, CultureInfo culture)
        {
            return name;
        }

        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return new List<LocalizedString>();
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            return new List<LocalizedString>();
        }
    }
}