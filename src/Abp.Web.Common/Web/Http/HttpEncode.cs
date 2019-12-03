using System.Text;
using static System.String;

namespace Abp.Web.Http
{
    public static class HttpEncode
    {
        public static bool JavaScriptEncodeAmpersand { get; set; }

        private static bool CharRequiresJavaScriptEncoding(char c)
        {
            return c < 0x20 // control chars always have to be encoded
                || c == '\"' // chars which must be encoded per JSON spec
                || c == '\\'
                || c == '\'' // HTML-sensitive chars encoded for safety
                || c == '<'
                || c == '>'
                || (c == '&' && JavaScriptEncodeAmpersand) // Bug Dev11 #133237. Encode '&' to provide additional security for people who incorrectly call the encoding methods (unless turned off by backcompat switch)
                || c == '\u0085' // newline chars (see Unicode 6.2, Table 5-1 [http://www.unicode.org/versions/Unicode6.2.0/ch05.pdf]) have to be encoded (DevDiv #663531)
                || c == '\u2028'
                || c == '\u2029';
        }

        private static void AppendCharAsUnicodeJavaScript(StringBuilder builder, char c)
        {
            builder.Append("\\u");
            builder.Append(((int)c).ToString("x4"));
        }

        public static string JavaScriptStringEncode(string value)
        {
            if (IsNullOrEmpty(value))
            {
                return Empty;
            }

            StringBuilder builder = null;
            var startIndex = 0;
            var count = 0;
            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (CharRequiresJavaScriptEncoding(c))
                {
                    if (builder == null)
                    {
                        builder = new StringBuilder(value.Length + 5);
                    }

                    if (count > 0)
                    {
                        builder.Append(value, startIndex, count);
                    }

                    startIndex = i + 1;
                    count = 0;
                }

                switch (c)
                {
                    case '\r':
                        builder.Append("\\r");
                        break;
                    case '\t':
                        builder.Append("\\t");
                        break;
                    case '\"':
                        builder.Append("\\\"");
                        break;
                    case '\\':
                        builder.Append("\\\\");
                        break;
                    case '\n':
                        builder.Append("\\n");
                        break;
                    case '\b':
                        builder.Append("\\b");
                        break;
                    case '\f':
                        builder.Append("\\f");
                        break;
                    default:
                        if (CharRequiresJavaScriptEncoding(c))
                        {
                            AppendCharAsUnicodeJavaScript(builder, c);
                        }
                        else
                        {
                            count++;
                        }
                        break;
                }
            }

            if (builder == null)
            {
                return value;
            }

            if (count > 0)
            {
                builder.Append(value, startIndex, count);
            }

            return builder.ToString();
        }
    }
}
