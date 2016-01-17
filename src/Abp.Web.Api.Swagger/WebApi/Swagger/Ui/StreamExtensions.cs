using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Abp.WebApi.Swagger.Ui
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Replace key with value from stream
        /// </summary>
        /// <param name="stream">Currently stream</param>
        /// <param name="replacements">Key and Value (Template Parameters and Real Value)</param>
        /// <returns></returns>
        public static Stream FindAndReplace(this Stream stream, IDictionary<string, string> replacements)
        {
            var originalText = new StreamReader(stream).ReadToEnd();
            var outputBuilder = new StringBuilder(originalText);

            foreach (var replacement in replacements)
            {
                outputBuilder.Replace(replacement.Key, replacement.Value);
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(outputBuilder.ToString()));
        }
    }
}
