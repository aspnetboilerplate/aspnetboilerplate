using System.Collections.Generic;
using Abp.Domain.Entities;
using Microsoft.AspNet.Identity;

namespace Abp.Security.Roles
{
    /// <summary>
    /// Represents a role in an application.
    /// </summary>
    public class AbpRole : AuditedEntity, IRole<int>
    {
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
        public virtual IList<RolePermission> Permissions { get; set; }
    }
}
