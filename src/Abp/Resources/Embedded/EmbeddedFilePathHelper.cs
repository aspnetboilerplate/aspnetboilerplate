using System;
using System.Text;

namespace Abp.Resources.Embedded
{
    public static class EmbeddedResourcePathHelper
    {
        public static string NormalizePath(string fullPath)
        {
            return fullPath?.Replace("/", ".").TrimStart('.');
        }

        public static string EncodeAsResourcesPath(string subPath)
        {
            var builder = new StringBuilder(subPath.Length);

            // does the subpath contain directory portion - if so we need to encode it.
            var indexOfLastSeperator = subPath.LastIndexOf('/');
            if (indexOfLastSeperator != -1)
            {
                // has directory portion to encode.
                for (int i = 0; i <= indexOfLastSeperator; i++)
                {
                    var currentChar = subPath[i];

                    if (currentChar == '/')
                    {
                        if (i != 0) // omit a starting slash (/), encode any others as a dot
                        {
                            builder.Append('.');
                        }
                        continue;
                    }

                    if (currentChar == '-')
                    {
                        builder.Append('_');
                        continue;
                    }

                    builder.Append(currentChar);
                }
            }

            // now append (and encode as necessary) filename portion
            if (subPath.Length > indexOfLastSeperator + 1)
            {
                // has filename to encode
                for (int c = indexOfLastSeperator + 1; c < subPath.Length; c++)
                {
                    var currentChar = subPath[c];
                    // no encoding to do on filename - so just append
                    builder.Append(currentChar);
                }
            }

            return builder.ToString();
        }
    }
}