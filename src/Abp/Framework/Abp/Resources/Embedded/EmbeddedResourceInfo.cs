using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Assembly Assembly { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        /// <param name="assembly"></param>
        public EmbeddedResourceInfo(byte[] content, Assembly assembly)
        {
            Content = content;
            Assembly = assembly;
        }
    }
}