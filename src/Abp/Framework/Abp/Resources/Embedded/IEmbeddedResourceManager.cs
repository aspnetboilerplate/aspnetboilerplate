using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmbeddedResourceManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootPath"></param>
        /// <param name="assembly"></param>
        /// <param name="resourceNamespace"></param>
        void ExposeResources(string rootPath, Assembly assembly, string resourceNamespace);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fullResourcePath"></param>
        /// <returns></returns>
        EmbeddedResourceInfo GetResource(string fullResourcePath);
    }
}