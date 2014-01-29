using System;
using System.Threading;
using Abp.Domain.Entities;

namespace Abp.Security.Tenants
{
    /// <summary>
    /// Represents a tenant account. A tenant is used in the cloud to identify a seperated application in the system.
    /// </summary>
    public class Tenant : Entity
    {
        /// <summary>
        /// Company name of the tenant.
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// Subdomain for this tenant.
        /// </summary>
        public virtual string Subdomain { get; set; }

        /// <summary>
        /// Creation time of this tenant.
        /// </summary>
        public virtual DateTime CreationTime { get; set; } //TODO: Make IHasCreationTime interface!

        #region Static properties

        /// <summary>
        /// Gets current tenant id.
        /// </summary>
        public static int CurrentTenantId
        {
            get { return 1; } //TODO: Implement Tenant claim!
        }

        #endregion
    }
}
