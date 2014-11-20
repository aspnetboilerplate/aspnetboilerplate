using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// Stores needed information of an embedded resource.
    /// </summary>
    public class EmbeddedResourceInfo
    {
        /// <summary>
        /// Content of the resource file.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// The assembly that contains the resource.
        /// </summary>
        public Assembly Assembly { get; set; }

        internal EmbeddedResourceInfo(byte[] content, Assembly assembly)
        {
            Content = content;
            Assembly = assembly;
        }
    }
}