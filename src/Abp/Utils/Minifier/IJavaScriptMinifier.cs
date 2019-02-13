using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Utils.Minifier
{
    /// <summary>
    /// Interface to minify JavaScript code.
    /// </summary>
    public interface IJavaScriptMinifier
    {
        string Minify(string javaScriptCode);
    }
}
