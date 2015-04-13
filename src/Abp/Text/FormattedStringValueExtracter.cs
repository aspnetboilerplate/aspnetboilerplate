using System.Collections.Generic;
using Abp.Text.Formatting;

namespace Abp.Text
{
    public class FormattedStringValueExtracter
    {
        public ExtractionResult Extract(string str, string format, bool ignoreCase = false)
        {
            var result = new ExtractionResult();

            var tokens = new FormatStringTokenizer().Tokenize(format);

            return result;
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