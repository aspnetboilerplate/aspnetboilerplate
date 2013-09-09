using System.Globalization;

namespace Abp.Utils.Extensions
{
    internal static class StringExtensions
    {
        private static readonly CultureInfo EnglishCultureInfo = new CultureInfo("en-US");

        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToLower(EnglishCultureInfo);
            }

            return char.ToLower(str[0], EnglishCultureInfo) + str.Substring(1);
        }

        public static string ToPascalCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            if (str.Length == 1)
            {
                return str.ToUpper(EnglishCultureInfo);
            }

            return char.ToUpper(str[0], EnglishCultureInfo) + str.Substring(1);
        }
    }
}