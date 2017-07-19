using Abp.Dependency;
using Abp.Resources.Embedded;

namespace Abp.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceViewFileProvider : EmbeddedResourceFileProvider
    {
        public EmbeddedResourceViewFileProvider(IIocResolver iocResolver) 
            : base(iocResolver)
        {
        }

        protected override bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != "cshtml";
        }
    }
}