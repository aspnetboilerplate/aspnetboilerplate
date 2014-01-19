using System;
using System.Threading;
using Abp.Domain.Entities;
using Abp.Security;

namespace Abp.Tenants
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
        public virtual DateTime CreationTime { get; set; }

        #region Static properties

        /// <summary>
        /// Gets current tenant id.
        /// </summary>
        public static int CurrentTenantId
        {
            get
            {
                if (Thread.CurrentPrincipal == null)
                {
                    throw new ApplicationException("Thread.CurrentPrincipal is null!");
                }

                var identity = Thread.CurrentPrincipal.Identity as AbpIdentity;
                if (identity == null)
                {
                    throw new ApplicationException("Thread.CurrentPrincipal.Identity is not type of AbpIdentity!");
                }

                return identity.TenantId;
            }
        }

        #endregion
    }
}
