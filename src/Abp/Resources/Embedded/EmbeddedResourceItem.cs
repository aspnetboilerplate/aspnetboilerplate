using System;
using System.IO;
using System.Reflection;

namespace Abp.Resources.Embedded
{
    /// <summary>
    /// Stores needed information of an embedded resource.
    /// </summary>
    public class EmbeddedResourceItem
    {
        public string Name { get; }

        /// <summary>
        /// Content of the resource file.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// The assembly that contains the resource.
        /// </summary>
        public Assembly Assembly { get; set; }

        public DateTime LastModifiedUtc { get; }

        internal EmbeddedResourceItem(string name, byte[] content, Assembly assembly)
        {
            Name = name;
            Content = content;
            Assembly = assembly;
            LastModifiedUtc = Assembly.Location != null
                ? new FileInfo(Assembly.Location).LastWriteTimeUtc
                : DateTime.UtcNow;
        }
    }
}