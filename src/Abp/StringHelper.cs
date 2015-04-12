using System;
using System.Text.RegularExpressions;

namespace Abp
{
    /// <summary>
    /// String helper methods.
    /// </summary>
    public static class StringHelper
    {
        /* ParseExact and TryParseExact methods are copied from 
         * http://stackoverflow.com/questions/1410012/parsing-formatted-string
         * and modified */

        /// <summary>
        /// Extracts parametric values from a string with given format.
        /// 
        /// Example:
        ///  ParseExact("My name is Neo.", "My name is {0}." returns an array includes an element "Neo".
        /// 
        /// Throws <see cref="ArgumentException"/> if <see cref="format"/> is not compatible with <see cref="str"/>.
        /// </summary>
        /// <param name="str">String to extract values</param>
        /// <param name="format">Expected format</param>
        /// <returns>Extracted values.</returns>
        public static string[] ParseExact(
            string str,
            string format)
        {
            return ParseExact(str, format, false);
        }

        /// <summary>
        /// Extracts parametric values from a string with given format.
        /// 
        /// Example:
        ///  ParseExact("My name is Neo.", "My name is {0}." returns an array includes an element "Neo".
        /// 
        /// Throws <see cref="ArgumentException"/> if <see cref="format"/> is not compatible with <see cref="str"/>.
        /// </summary>
        /// <param name="str">String to extract values</param>
        /// <param name="format">Expected format</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>Extracted values.</returns>
        public static string[] ParseExact(
            string str,
            string format,
            bool ignoreCase)
        {
            string[] values;

            if (!TryParseExact(str, format, out values, ignoreCase))
            {
                throw new ArgumentException("format is not compatible with str.");
            }

            return values;
        }


        /// <summary>
        /// Tries to Extract parametric values from a string with given format.
        /// Example:
        /// ParseExact("My name is Neo.", "My name is {0}." gets an array includes an element "Neo".
        /// </summary>
        /// <param name="str">String to extract values</param>
        /// <param name="format">Expected format</param>
        /// <param name="values">Extracted values</param>
        /// <returns>True if all parameters are extracted.</returns>
        public static bool TryParseExact(
            string str,
            string format,
            out string[] values)
        {
            return TryParseExact(str, format, out values, false);
        }

        /// <summary>
        /// Tries to Extract parametric values from a string with given format.
        /// Example:
        /// ParseExact("My name is Neo.", "My name is {0}." gets an array includes an element "Neo".
        /// </summary>
        /// <param name="str">String to extract values</param>
        /// <param name="format">Expected format</param>
        /// <param name="values">Extracted values</param>
        /// <param name="ignoreCase">Ignore case</param>
        /// <returns>True if all parameters are extracted.</returns>
        public static bool TryParseExact(
            string str,
            string format,
            out string[] values,
            bool ignoreCase)
        {
            format = Regex.Escape(format).Replace("\\{", "{");

            int tokenCount;
            for (tokenCount = 0; ; tokenCount++)
            {
                var token = string.Format("{{{0}}}", tokenCount);
                if (!format.Contains(token))
                {
                    break;
                }

                format = format.Replace(token, string.Format("(?'group{0}'.*)", tokenCount));
            }

            var regexOptions = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            var match = new Regex(format, regexOptions).Match(str);

            if (tokenCount != (match.Groups.Count - 1))
            {
                values = new string[] { };
                return false;
            }
            
            values = new string[tokenCount];
            for (var i = 0; i < tokenCount; i++)
            {
                values[i] = match.Groups[string.Format("group{0}", i)].Value;
            }

            return true;
        }
    }
}