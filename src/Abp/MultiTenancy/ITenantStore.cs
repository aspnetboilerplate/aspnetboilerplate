using JetBrains.Annotations;

namespace Abp.MultiTenancy
{
    public interface ITenantStore
    {
        [CanBeNull]
        TenantInfo Find(long tenantId);

        [CanBeNull]
        TenantInfo Find([NotNull] string tenancyName);
    }
}