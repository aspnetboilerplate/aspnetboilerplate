using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceInfo
    {
        public byte[] Content { get; set; }

        public Assembly Assembly { get; set; }

        public EmbeddedResourceInfo(byte[] content, Assembly assembly)
        {
            Content = content;
            Assembly = assembly;
        }
    }
}