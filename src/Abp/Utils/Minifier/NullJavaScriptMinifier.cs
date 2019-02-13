using System;
using System.Collections.Generic;
using System.Text;
using Castle.Core.Logging;

namespace Abp.Utils.Minifier
{
    /// <summary>
    /// Null pattern implementation of <see cref="IJavaScriptMinifier"/>.
    /// </summary>
    public class NullJavaScriptMinifier : IJavaScriptMinifier
    {
        /// <summary>
        /// Gets single instance of <see cref="NullJavaScriptMinifier"/> class.
        /// </summary>
        public static NullJavaScriptMinifier Instance { get; } = new NullJavaScriptMinifier();

        public ILogger Logger { get; set; }

        public NullJavaScriptMinifier()
        {
            Logger = NullLogger.Instance;
        }

        public string Minify(string javaScriptCode)
        {
            Logger.Warn("USING NullJavaScriptMinifier!");

            return javaScriptCode;
        }
    }
}
