namespace Abp.Web.Minifier
{
    /// <summary>
    /// Interface to minify JavaScript code.
    /// </summary>
    public interface IJavaScriptMinifier
    {
        string Minify(string javaScriptCode);
    }
}
