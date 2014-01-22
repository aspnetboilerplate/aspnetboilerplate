using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Tenants;

namespace Abp.Roles
{
    /// <summary>
    /// Represents a role in an application.
    /// </summary>
    public class Role : AuditedEntity, IHasTenant
    {
        /// <summary>
        /// Tenant of this role.
        /// </summary>
        public virtual Tenant Tenant { get; set; }

        /// <summary>
        /// Unique name of this role.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name of this role.
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// List of permissions of this role.
        /// </summary>
        public virtual List<RolePermission> Permissions { get; set; }
    }
}
