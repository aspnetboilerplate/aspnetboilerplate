using System.IO;
using System.Web.Hosting;
using Abp.Resources.Embedded;

namespace Abp.Web.Mvc.Resources.Embedded
{
    public class EmbeddedResourceVirtualFile : VirtualFile
    {
        private readonly EmbeddedResourceItem _resourceItem;

        public EmbeddedResourceVirtualFile(string virtualPath, EmbeddedResourceItem resourceItem) : base(virtualPath)
        {
            _resourceItem = resourceItem;
        }

        public override Stream Open()
        {
            return new MemoryStream(_resourceItem.Content);
        }
    }
}
