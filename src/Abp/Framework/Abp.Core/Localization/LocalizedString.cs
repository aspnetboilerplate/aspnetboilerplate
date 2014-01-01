using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// Represents a localized string.
    /// </summary>
    public class LocalizedString
    {
        /// <summary>
        /// Culture info for this string.
        /// </summary>
        public CultureInfo CultureInfo { get; private set; }

        /// <summary>
        /// Unique Name of the string.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Value for the <see cref="Name"/>.
        /// </summary>
        public string Value { get; private set; }

        /// <param name="cultureInfo">Culture info for this string</param>
        /// <param name="name">Unique Name of the string</param>
        /// <param name="value">Value for the <see cref="name"/></param>
        public LocalizedString(CultureInfo cultureInfo, string name, string value)
        {
            CultureInfo = cultureInfo;
            Name = name;
            Value = value;
        }
    }
}