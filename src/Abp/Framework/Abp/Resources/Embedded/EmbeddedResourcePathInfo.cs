using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourcePathInfo
    {
        public string Path { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public Assembly Assembly { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string ResourceNamespace { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceNamespace"></param>
        public EmbeddedResourcePathInfo(string path, Assembly assembly, string resourceNamespace)
        {
            Path = path;
            Assembly = assembly;
            ResourceNamespace = resourceNamespace;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public string FindManifestName(string fullPath)
        {
            var relativeResourcePath = fullPath.Substring(Path.Length + 1);
            relativeResourcePath = NormalizeResourcePath(relativeResourcePath);
            return String.Format("{0}.{1}", ResourceNamespace, relativeResourcePath.Replace("/", "."));
        }

        private static string NormalizeResourcePath(string path)
        {
            var pathFolders = path.Split('/');
            if (pathFolders.Length < 2)
            {
                return path;
            }

            var stringBuilder = new StringBuilder();

            for (var i = 0; i < pathFolders.Length - 1; i++)
            {
                stringBuilder.Append(NormalizeFolderName(pathFolders[i]) + "/");
            }

            stringBuilder.Append(pathFolders.Last()); //Append file name

            return stringBuilder.ToString();
        }

        private static string NormalizeFolderName(string pathPart)
        {
            string regexSearch = new string(System.IO.Path.GetInvalidFileNameChars()) + new string(System.IO.Path.GetInvalidFileNameChars());

            Regex regexPattern = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
            return regexPattern.Replace(pathPart, string.Empty);
        }
    }
}