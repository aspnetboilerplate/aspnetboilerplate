using System.Globalization;

namespace Abp.Extensions
{
    /// <summary>
    /// Extension methods for String class.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Default used CultureInfo.
        /// </summary>
        private static readonly CultureInfo DefaultCultureInfo = new CultureInfo("en-US");

        /// <summary>
        /// Converts PascalCase string to camelCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str)
        {
            return str.ToCamelCase(DefaultCultureInfo);
        }

        /// <summary>
        /// Converts PascalCase string to camelCase string in specified culture.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>camelCase of the string</returns>
        public static string ToCamelCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLower(culture);
            }

            return char.ToLower(str[0], culture) + str.Substring(1);
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str)
        {
            return str.ToPascalCase(DefaultCultureInfo);
        }

        /// <summary>
        /// Converts camelCase string to PascalCase string in specified culture.
        /// </summary>
        /// <param name="str">String to convert</param>
        /// <param name="culture">An object that supplies culture-specific casing rules</param>
        /// <returns>PascalCase of the string</returns>
        public static string ToPascalCase(this string str, CultureInfo culture)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper(culture);
            }

            return char.ToUpper(str[0], culture) + str.Substring(1);
        }
    }
}