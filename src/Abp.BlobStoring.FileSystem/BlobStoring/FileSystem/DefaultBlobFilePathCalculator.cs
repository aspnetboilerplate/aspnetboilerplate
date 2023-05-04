using System.IO;
using Abp.BlobStoring;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.BlobStoring.FileSystem
{
    public class DefaultBlobFilePathCalculator : IBlobFilePathCalculator, ITransientDependency
    {
        protected IAbpSession CurrentTenant { get; }

        public DefaultBlobFilePathCalculator(IAbpSession currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            var fileSystemConfiguration = args.Configuration.GetFileSystemConfiguration();
            var blobPath = fileSystemConfiguration.BasePath;

            blobPath = CurrentTenant.TenantId == null
                ? Path.Combine(blobPath, "host")
                : Path.Combine(blobPath, "tenants", CurrentTenant.TenantId.Value.ToString("D"));

            if (fileSystemConfiguration.AppendContainerNameToBasePath)
            {
                blobPath = Path.Combine(blobPath, args.ContainerName);
            }

            blobPath = Path.Combine(blobPath, args.BlobName);

            return blobPath;
        }
    }
}
