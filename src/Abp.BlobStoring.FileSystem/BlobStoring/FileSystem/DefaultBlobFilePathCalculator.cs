using System.IO;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.BlobStoring.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator, ITransientDependency
    {
        protected IAbpSession AbpSession { get; }

        public DefaultBlobFilePathCalculator(IAbpSession session)
        {
            AbpSession = session;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var blobPath = fileSystemConfiguration.BasePath;

            blobPath = AbpSession.TenantId == null
                ? Path.Combine(blobPath, "host")
                : Path.Combine(blobPath, "tenants", AbpSession.TenantId.Value.ToString("D"));

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}
