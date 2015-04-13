using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Text.Formatting;

namespace Abp.Text
{
    public class FormattedStringValueExtracter
    {
        public ExtractionResult Extract(string str, string format, bool ignoreCase = false)
        {
            var stringComparison = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            if (str == format) //TODO: think on that!
            {
                return new ExtractionResult(true);
            }

            var formatTokens = new FormatStringTokenizer().Tokenize(format);
            if (formatTokens.IsNullOrEmpty())
            {
                return new ExtractionResult(str == "");
            }

            var result = new ExtractionResult(true);

            for (var i = 0; i < formatTokens.Count; i++)
            {
                var currentToken = formatTokens[i];
                var previousToken = i > 0 ? formatTokens[i - 1] : null;

                if (currentToken.Type == FormatStringTokenType.ConstantText)
                {
                    if (i == 0)
                    {
                        if (!str.StartsWith(currentToken.Text, stringComparison))
                        {
                            result.IsMatched = false;
                            return result;
                        }
                        
                        str = str.Substring(currentToken.Text.Length);
                    }
                    else
                    {
                        var matchIndex = str.IndexOf(currentToken.Text, stringComparison);
                        if (matchIndex < 0)
                        {
                            result.IsMatched = false;
                            return result;
                        }

                        Debug.Assert(previousToken != null, "previousToken can not be null since i > 0 here");
                        
                        result.Matches.Add(new FormattedStringDynamicValueMatch(previousToken.Text, str.Substring(0, matchIndex)));
                        str = str.Substring(matchIndex + currentToken.Text.Length);
                    }
                }
            }

            var lastToken = formatTokens.Last();
            if (lastToken.Type == FormatStringTokenType.DynamicValue)
            {
                result.Matches.Add(new FormattedStringDynamicValueMatch(lastToken.Text, str));
            }

            return result;
        }


        public class ExtractionResult
        {
            public bool IsMatched { get; set; }

            public List<FormattedStringDynamicValueMatch> Matches { get; private set; }

            public ExtractionResult(bool isMatched)
            {
                IsMatched = isMatched;
                Matches = new List<FormattedStringDynamicValueMatch>();
            }
        }
    }

    public class FormattedStringDynamicValueMatch
    {
        public string Name { get; private set; }

        public string Value { get; private set; }

        public FormattedStringDynamicValueMatch(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}