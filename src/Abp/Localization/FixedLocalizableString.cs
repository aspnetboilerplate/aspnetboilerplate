using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// A class that gets the same string on every localization.
    /// </summary>
    public class FixedLocalizableString : ILocalizableString
    {
        /// <summary>
        /// The fixed string.
        /// Whenever Localize methods called, this string is returned.
        /// </summary>
        public virtual string FixedString { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="FixedLocalizableString"/>.
        /// </summary>
        /// <param name="fixedString">
        /// The fixed string.
        /// Whenever Localize methods called, this string is returned.
        /// </param>
        public FixedLocalizableString(string fixedString)
        {
            FixedString = fixedString;
        }

        /// <summary>
        /// Gets the <see cref="FixedString"/> always.
        /// </summary>
        public virtual string Localize()
        {
            return FixedString;
        }

        /// <summary>
        /// Gets the <see cref="FixedString"/> always.
        /// </summary>
        public virtual string Localize(CultureInfo culture)
        {
            return FixedString;
        }

        public override string ToString()
        {
            return FixedString;
        }
    }
}