using System.Reflection;

namespace Abp.Web.Mvc.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEmbeddedResourceManager
    {
        void Expose(string resourceName, Assembly assembly, string resourceNamespace);
        void Initialize();
    }
}