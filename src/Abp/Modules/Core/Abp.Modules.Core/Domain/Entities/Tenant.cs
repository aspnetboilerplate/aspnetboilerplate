using System;
using System.Threading;
using Abp.Modules.Core.Domain.Entities.Utils;
using Abp.Security;

namespace Abp.Modules.Core.Domain.Entities
{
    /// <summary>
    /// Represents a tenant account. A tenant is used in the cloud to identify a seperated application in the system.
    /// </summary>
    public class Tenant : AuditedEntity
    {
        /// <summary>
        /// Company name
        /// </summary>
        public virtual string CompanyName { get; set; }

        /// <summary>
        /// Tenant owner.
        /// </summary>
        public virtual User Owner { get; set; }

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
