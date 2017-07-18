using JetBrains.Annotations;

namespace Abp.MultiTenancy
{
    public interface ITenantStore
    {
        [CanBeNull]
        TenantInfo Find(int tenantId);

        [CanBeNull]
        TenantInfo Find([NotNull] string tenancyName);
    }
}