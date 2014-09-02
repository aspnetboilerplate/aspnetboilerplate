using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmbeddedResourceManager
    {
        void ExposeResources(string rootPath, Assembly assembly, string resourceNamespace);

        EmbeddedResourceInfo GetResource(string fullResourcePath);
    }
}