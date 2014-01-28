using Abp.Domain.Entities;

namespace Abp.Security.Roles
{
    /// <summary>
    /// Represents a permission for a role.
    /// Used to grant/deny a permission for a role.
    /// </summary>
    public class RolePermission : CreationAuditedEntity
    {
        /// <summary>
        /// The role.
        /// </summary>
        public virtual Role Role { get; set; }

        /// <summary>
        /// Unique name of the permission.
        /// </summary>
        public virtual string PermissionName { get; set; }

        /// <summary>
        /// Is this role granted for this permission.
        /// Default value: true.
        /// </summary>
        public virtual bool IsGranted { get; set; }

        /// <summary>
        /// Creates a new <see cref="RolePermission"/> instance.
        /// </summary>
        public RolePermission()
        {
            IsGranted = true;
        }
    }
}
