namespace Abp.MultiTenancy
{
    /// <summary>
    /// Used to get current tenant id.
    /// This interface can be implemented to get Tenant's Id if user has not logged in.
    /// It can resolve TenantId from subdomain, for example.
    /// </summary>
    public interface ITenantIdResolver
    {
        /// <summary>
        /// Gets current TenantId or null.
        /// </summary>
        int? TenantId { get; }
    }
}