using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    public abstract class PermissionSettingEntityBase : Entity, ICreationAudited
    {
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
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user of this entity.
        /// </summary>
        public virtual AbpUser CreatorUser { get; set; }
        
        /// <summary>
        /// Creates a new <see cref="RolePermission"/> instance.
        /// </summary>
        protected PermissionSettingEntityBase()
        {
            IsGranted = true;
        }
    }
}