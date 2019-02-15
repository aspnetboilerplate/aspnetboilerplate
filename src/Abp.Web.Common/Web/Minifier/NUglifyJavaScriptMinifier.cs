using Castle.Core.Logging;
using NUglify;

namespace Abp.Web.Minifier
{
    public class NUglifyJavaScriptMinifier : IJavaScriptMinifier
    {
        public ILogger Logger { get; set; }

        public NUglifyJavaScriptMinifier()
        {
            Logger = NullLogger.Instance;
        }

        public string Minify(string javaScriptCode)
        {
            Check.NotNull(javaScriptCode, nameof(javaScriptCode));

            var result = Uglify.Js(javaScriptCode);
            if (!result.HasErrors)
            {
                return result.Code;
            }

            Logger.Warn($"{nameof(NUglifyJavaScriptMinifier)} has encountered an error in handling javascript.");
            result.Errors.ForEach(error => Logger.Warn(error.ToString()));

            return javaScriptCode;
        }
    }
}
