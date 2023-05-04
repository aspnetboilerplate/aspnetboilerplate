using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.BlobStoring.Azure
{
    public class DefaultAzureBlobNameCalculator : IAzureBlobNameCalculator, ITransientDependency
    {
        protected IAbpSession CurrentTenant { get; }

        public DefaultAzureBlobNameCalculator(IAbpSession currentTenant)
        {
            CurrentTenant = currentTenant;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            return CurrentTenant.TenantId == null
                ? $"host/{args.BlobName}"
                : $"tenants/{CurrentTenant.TenantId.Value:D}/{args.BlobName}";
        }
    }
}