using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abp.WebApi.Controllers.Dynamic.Formatters
{
    /// <summary>
    /// This class is used to return plain text reponse from <see cref="ApiController"/>s.
    /// </summary>
    public class PlainTextFormatter : MediaTypeFormatter
    {
        /// <summary>
        /// Creates a new <see cref="PlainTextFormatter"/> object.
        /// </summary>
        public PlainTextFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/plain"));
        }

        public override bool CanReadType(Type type)
        {
            return type == typeof(string);
        }

        public override bool CanWriteType(Type type)
        {
            return type == typeof(string);
        }

        public override Task<object> ReadFromStreamAsync(Type type, Stream stream, HttpContent content, IFormatterLogger formatterLogger)
        {
            string value;
            using (var reader = new StreamReader(stream))
            {
                value = reader.ReadToEnd();
            }

            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(value);
            return tcs.Task;
        }

        public override Task WriteToStreamAsync(Type type, object value, Stream stream, HttpContent content, TransportContext transportContext)
        {
            using (var writer = new StreamWriter(stream))
            {
                writer.Write((string)value);
                writer.Flush();
            }

            var tcs = new TaskCompletionSource<object>();
            tcs.SetResult(null);
            return tcs.Task;
        }
    }
}