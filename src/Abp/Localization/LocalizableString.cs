using System;
using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// Represents a string that can be localized.
    /// </summary>
    [Serializable]
    public class LocalizableString : ILocalizableString
    {
        /// <summary>
        /// Unique name of the localization source.
        /// </summary>
        public virtual string SourceName { get; private set; }

        /// <summary>
        /// Unique Name of the string to be localized.
        /// </summary>
        public virtual string Name { get; private set; }

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private LocalizableString()
        {
            
        }

        /// <param name="name">Unique Name of the string to be localized</param>
        /// <param name="sourceName">Unique name of the localization source</param>
        public LocalizableString(string name, string sourceName)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (sourceName == null)
            {
                throw new ArgumentNullException("sourceName");
            }

            Name = name;
            SourceName = sourceName;
        }

        public string Localize(ILocalizationContext context)
        {
            return context.LocalizationManager.GetString(SourceName, Name);
        }

        public string Localize(ILocalizationContext context, CultureInfo culture)
        {
            return context.LocalizationManager.GetString(SourceName, Name, culture);
        }

        public override string ToString()
        {
            return string.Format("[LocalizableString: {0}, {1}]", Name, SourceName);
        }
    }
}
