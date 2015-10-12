using Abp.Runtime.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Yishe.Abp.Util.Help
{
    public static class StringHelper
    {
        public static Guid NewCombGuid()
        {
            byte[] guidArray = Guid.NewGuid().ToByteArray();
            DateTime baseDate = new DateTime(1900, 1, 1);
            DateTime now = DateTime.Now;
            // Get the days and milliseconds which will be used to build the byte string 
            TimeSpan days = new TimeSpan(now.Ticks - baseDate.Ticks);
            TimeSpan msecs = new TimeSpan(now.Ticks - (new DateTime(now.Year, now.Month, now.Day).Ticks));
            // Convert to a byte array 
            // Note that SQL Server is accurate to 1/300th of a millisecond so we divide by 3.333333 
            byte[] daysArray = BitConverter.GetBytes(days.Days);
            byte[] msecsArray = BitConverter.GetBytes((long)(msecs.TotalMilliseconds / 3.333333));
            // Reverse the bytes to match SQL Servers ordering 
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);
            // Copy the bytes into the guid 
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);
            return new System.Guid(guidArray);
        }

        /// <summary>
        /// 将Guid转换为经过Base64编码的22位字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string FromGuid(Guid source)
        {
            string base64 = Convert.ToBase64String(source.ToByteArray());

            string result = base64.Replace("/", "_").Replace("+", "-").Substring(0, 22);

            return result;
        }

        /// <summary>
        /// 将经过Base64编码的22位字符串还原为Guid
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Guid Base64ToGuid(string source)
        {
            Guid result = Guid.Empty;
            source = source.Trim();
            string encoded = string.Concat(source.Trim().Replace("-", "+").Replace("_", "/"), "==");

            try
            {
                byte[] base64 = Convert.FromBase64String(encoded);
                result = new Guid(base64);
            }
            catch (Exception ex)
            {
                throw new AbpValidationException("不是有效的参数格式", ex);
            }

            return result;
        }

        public static string CleanSearchString(string searchString)
        {
            if (string.IsNullOrEmpty(searchString))
            {
                return "";
            }
            searchString = searchString.Replace("*", "%");
            searchString = ReplaceBadChar(searchString);
            searchString = StripHtmlXmlTags(searchString);
            searchString = Regex.Replace(searchString, "--|;|'|\"", " ", RegexOptions.Compiled | RegexOptions.Multiline);
            return searchString;
        }

        /// <summary>
        /// 去除非法字串
        /// </summary>
        /// <param name="strChar">原字串</param>
        /// <returns>过滤过的字串</returns>
        public static string ReplaceBadChar(string strChar)
        {
            if (strChar.Trim() == "")
            {
                return "";
            }
            else
            {
                strChar = strChar.Replace("'", "");
                strChar = strChar.Replace("*", "");
                strChar = strChar.Replace("?", "");
                //strChar = strChar.Replace("(", "");
                //strChar = strChar.Replace(")", "");
                strChar = strChar.Replace("<", "");
                strChar = strChar.Replace("=", "");
                return strChar.Trim();
            }
        }

        public static string StripAllTags(string strToStrip)
        {
            strToStrip = Regex.Replace(strToStrip, @"</p(?:\s*)>(?:\s*)<p(?:\s*)>", "\n\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            strToStrip = Regex.Replace(strToStrip, @"<br(?:\s*)/>", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            strToStrip = Regex.Replace(strToStrip, "\"", "''", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            strToStrip = StripHtmlXmlTags(strToStrip);
            return strToStrip;
        }

        public static string StripForPreview(string content)
        {
            content = Regex.Replace(content, "<br>", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<br/>", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<br />", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "<p>", "\n", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            content = content.Replace("'", "&#39;");
            return StripHtmlXmlTags(content);
        }

        public static string StripHtmlXmlTags(string content)
        {
            return Regex.Replace(content, "<[^>]+>", "", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        public static string StripScriptTags(string content)
        {
            content = Regex.Replace(content, "<script((.|\n)*?)</script>", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            content = Regex.Replace(content, "'javascript:", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return Regex.Replace(content, "\"javascript:", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }


        public static string HtmlDecode(string formattedPost)
        {
            RegexOptions options = RegexOptions.IgnoreCase;
            formattedPost = Regex.Replace(formattedPost, "&lt;", "<", options);
            formattedPost = Regex.Replace(formattedPost, "&gt;", ">", options);
            return formattedPost;
        }

        public static string HtmlEncode(string formattedPost)
        {
            formattedPost = Regex.Replace(formattedPost, "<", "&lt;");
            formattedPost = Regex.Replace(formattedPost, ">", "&gt;");
            return formattedPost;
        }

    }
}
