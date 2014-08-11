using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Security.Permissions
{
    /// <summary>
    /// Represents a permission for a role.
    /// Used to grant/deny a permission for a role.
    /// </summary>
    //[Table("AbpPermissions")]
    public class Permission : Entity<long>, ICreationAudited
    {
        /// <summary>
        /// Role Id.
        /// </summary>
        public virtual int? RoleId { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual long? UserId { get; set; }

        /// <summary>
        /// Unique name of the permission.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Is this role granted for this permission.
        /// Default value: true.
        /// </summary>
        public virtual bool IsGranted { get; set; }

        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user of this entity.
        /// </summary>
        public virtual long? CreatorUserId { get; set; }

        public Permission()
        {
            IsGranted = true;
        }
    }
}
