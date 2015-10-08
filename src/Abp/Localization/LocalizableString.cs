using System;
using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// Represents a string that can be localized.
    /// </summary>
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

        /// <param name="name">Unique name of the localization source</param>
        /// <param name="sourceName">Unique Name of the string to be localized</param>
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

        /// <summary>
        /// Localizes the string in current language.
        /// </summary>
        /// <returns>Localized string</returns>
        public virtual string Localize()
        {
            return LocalizationHelper.GetString(SourceName, Name);
        }

        /// <summary>
        /// Localizes the string in current language.
        /// </summary>
        /// <param name="culture">culture</param>
        /// <returns>Localized string</returns>
        public virtual string Localize(CultureInfo culture)
        {
            return LocalizationHelper.GetString(SourceName, Name, culture);
        }

        //public override string ToString()
        //{
        //    return Localize();
        //}
    }
}