using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Abp.Dependency;
using Abp.Logging;
using Abp.Resources.Embedded;

namespace Abp.Web.Mvc.Extensions
{
    public static class HtmlHelperResourceExtensions
    {
        private static readonly ConcurrentDictionary<string, string> Cache;
        private static readonly object SyncObj = new object();

        static HtmlHelperResourceExtensions()
        {
            Cache = new ConcurrentDictionary<string, string>();
        }

        /// <summary>
        /// Includes a script to the page with versioning.
        /// </summary>
        /// <param name="html">Reference to the HtmlHelper object</param>
        /// <param name="url">URL of the script file</param>
        public static IHtmlString IncludeScript(this HtmlHelper html, string url)
        {
            return html.Raw("<script src=\"" + GetPathWithVersioning(url) + "\" type=\"text/javascript\"></script>");
        }

        /// <summary>
        /// Includes a style to the page with versioning.
        /// </summary>
        /// <param name="html">Reference to the HtmlHelper object</param>
        /// <param name="url">URL of the style file</param>
        public static IHtmlString IncludeStyle(this HtmlHelper html, string url)
        {
            return html.Raw("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + GetPathWithVersioning(url) + "\" />");
        }

        private static string GetPathWithVersioning(string path)
        {
            if (Cache.ContainsKey(path))
            {
                return Cache[path];
            }

            lock (SyncObj)
            {
                if (Cache.ContainsKey(path))
                {
                    return Cache[path];
                }

                string result;
                try
                {
                    // CDN resource
                    if (path.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase) || path.StartsWith("//", StringComparison.CurrentCultureIgnoreCase))
                    {
                        //Replace "http://" from beginning
                        result = Regex.Replace(path, @"^http://", "//", RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        var fullPath = HttpContext.Current.Server.MapPath(path.Replace("/", "\\"));
                        result = File.Exists(fullPath)
                            ? GetPathWithVersioningForPhysicalFile(path, fullPath)
                            : GetPathWithVersioningForEmbeddedFile(path);
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.Logger.Error("Can not find file for: " + path + "! " + ex.ToString());
                    result = path;
                }

                Cache[path] = result;
                return result;
            }
        }

        private static string GetPathWithVersioningForPhysicalFile(string path, string filePath)
        {
            var fileVersion = new FileInfo(filePath).LastWriteTime.Ticks;
            return VirtualPathUtility.ToAbsolute(path) + "?v=" + fileVersion;
        }

        private static string GetPathWithVersioningForEmbeddedFile(string path)
        {
            //Remove "~/" from beginning
            var embeddedResourcePath = path;

            if (embeddedResourcePath.StartsWith("~"))
            {
                embeddedResourcePath = embeddedResourcePath.Substring(1);
            }

            if (embeddedResourcePath.StartsWith("/"))
            {
                embeddedResourcePath = embeddedResourcePath.Substring(1);
            }

            var resource = SingletonDependency<IEmbeddedResourceManager>.Instance.GetResource(embeddedResourcePath);
            var fileVersion = new FileInfo(resource.Assembly.Location).LastWriteTime.Ticks;
            return VirtualPathUtility.ToAbsolute(path) + "?v=" + fileVersion;
        }
    }
}
