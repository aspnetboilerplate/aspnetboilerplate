using System.Linq;
using System.Reflection;
using System.Text;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    internal class EmbeddedResourcePathInfo
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

        public string FindManifestName(string fullPath)
        {
            var relativeResourcePath = fullPath.Substring(Path.Length + 1);
            relativeResourcePath = NormalizeResourcePath(relativeResourcePath);
            return string.Format("{0}.{1}", ResourceNamespace, relativeResourcePath.Replace("/", "."));
        }

        private static string NormalizeResourcePath(string path)
        {
            var pathFolders = path.Split('/');
            if (pathFolders.Length < 2)
            {
                return path;
            }
            
            var sb = new StringBuilder();

            for (var i = 0; i < pathFolders.Length - 1; i++)
            {
                sb.Append(NormalizeFolderName(pathFolders[i]) + "/");
            }

            sb.Append(pathFolders.Last()); //Append file name

            return sb.ToString();
        }

        private static string NormalizeFolderName(string pathPart)
        {
            //TODO: Implement all rules of .NET
            return pathPart.Replace('-', '_');
        }
    }
}