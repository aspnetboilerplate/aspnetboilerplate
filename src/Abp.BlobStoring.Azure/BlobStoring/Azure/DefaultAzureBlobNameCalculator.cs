using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.BlobStoring.Azure
{
    public class DefaultAzureBlobNameCalculator : IAzureBlobNameCalculator, ITransientDependency
    {
        protected IAbpSession AbpSession { get; }

        public DefaultAzureBlobNameCalculator(IAbpSession session)
        {
            AbpSession = session;
        }

        public virtual string Calculate(BlobProviderArgs args)
        {
            return AbpSession.TenantId == null
                ? $"host/{args.BlobName}"
                : $"tenants/{AbpSession.TenantId.Value:D}/{args.BlobName}";
        }
    }
}