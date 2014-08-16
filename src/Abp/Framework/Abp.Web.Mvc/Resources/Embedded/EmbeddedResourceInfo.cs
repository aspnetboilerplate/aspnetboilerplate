namespace Abp.Resources.Embedded
{
    /// <summary>
    /// 
    /// </summary>
    public class EmbeddedResourceInfo
    {
        public string Name { get; set; }

        public byte[] Content { get; set; }

        public string MimeType { get; set; }

        public EmbeddedResourceInfo(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }

        public EmbeddedResourceInfo(string name, byte[] content, string mimeType)
            : this(name, content)
        {
            MimeType = mimeType;
        }
    }
}