using System;
using System.Reflection;

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

        public string FindManifestName(string fullPath)
        {
            return string.Format("{0}.{1}", ResourceNamespace, fullPath.Substring(Path.Length + 1).Replace("/", "."));
        }
    }
}