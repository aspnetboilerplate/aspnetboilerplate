using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Security.Tenants
{
    /// <summary>
    /// Represents a Tenant of the application.
    /// </summary>
    public class AbpTenant : Entity, IHasCreationTime
    {
        /// <summary>
        /// Name of the Tenant.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Creation time of this Tenant.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Gets current Tenant's Id of null if no Tenant.
        /// </summary>
        public static int? CurrentTenantId
        {
            get { return null; } //TODO: Add Claim to Identity and check for it!
        }
    }
}
