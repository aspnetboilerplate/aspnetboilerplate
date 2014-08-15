using System.Reflection;

namespace Abp.Web.Mvc.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceInfo
    {
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
        /// <param name="assembly"></param>
        /// <param name="resourceNamespace"></param>
        public EmbeddedResourceInfo(Assembly assembly, string resourceNamespace)
        {
            Assembly = assembly;
            ResourceNamespace = resourceNamespace;
        }
    }
}