using System;
using System.Collections.Generic;

namespace Abp.Strings
{
    public class FormattedStringValueExtracter
    {
        public ExtractionResult Extract(string str, string format, bool ignoreCase = false)
        {
            var result = new ExtractionResult();

            var tokens = TokenizeFormat(format);


            return result;
        }

        private List<FormatToken> TokenizeFormat(string format)
        {
            var tokens = new List<FormatToken>();

            var currentText = "";

            var inFormatValue = false;
            for (var i = 0; i < format.Length; i++)
            {
                var c = format[i];
                switch (c)
                {
                    case '{':
                        if (inFormatValue)
                        {
                            throw new FormatException("Can not contain nested format expression! Index: " + i);
                        }

                        inFormatValue = true;

                        if (currentText.Length > 0)
                        {
                            tokens.Add(new FormatToken(currentText, false));
                        }

                        currentText = "";
                        
                        break;
                    case '}':
                        if (!inFormatValue)
                        {
                            throw new FormatException("These is no opening { for the }. Index: " + i);                            
                        }

                        inFormatValue = false;
                        
                        tokens.Add(new FormatToken(currentText, true));
                        
                        break;
                    default:
                        currentText += c;
                        break;
                }
            }

            if (currentText.Length > 0)
            {
                tokens.Add(new FormatToken(currentText, false));
            }

            return tokens;
        }

        private class FormatToken
        {
            public FormatToken(string text, bool isDynamic)
            {
                Text = text;
                IsDynamic = isDynamic;
            }

            public string Text { get; set; }
            
            public bool IsDynamic { get; set; }
        }

        public class ExtractionResult
        {
            public bool Matched { get; set; }

            public List<string> Values { get; set; }

            public ExtractionResult()
            {
                Values = new List<string>();
            }
        }
    }
}