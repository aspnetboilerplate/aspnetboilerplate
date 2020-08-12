using Abp.Domain.Uow;

namespace Abp.MultiTenancy
{
    public class DbPerTenantConnectionStringResolveArgs : ConnectionStringResolveArgs
    {
        public long? TenantId { get; set; }

        public DbPerTenantConnectionStringResolveArgs(long? tenantId, MultiTenancySides? multiTenancySide = null)
            : base(multiTenancySide)
        {
            TenantId = tenantId;
        }

        public DbPerTenantConnectionStringResolveArgs(long? tenantId, ConnectionStringResolveArgs baseArgs)
        {
            TenantId = tenantId;
            MultiTenancySide = baseArgs.MultiTenancySide;

            foreach (var kvPair in baseArgs)
            {
                Add(kvPair.Key, kvPair.Value);
            }
        }
    }
}