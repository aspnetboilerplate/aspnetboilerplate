using Abp.Domain.Entities;

namespace Abp.Security.Tenants
{
    /// <summary>
    /// Represents a Tenant of the application.
    /// </summary>
    public class AbpTenant : Entity
    {
        /// <summary>
        /// Name of the Tenant.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets current Tenant's Id of null if no Tenant.
        /// </summary>
        public static int? CurrentTenantId
        {
            get { return null; } //TODO: Add Claim to Identity and check for it!
        }
    }
}
