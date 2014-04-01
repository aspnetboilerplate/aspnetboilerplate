using System;
using System.Data;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Security.Users;

namespace Abp.Security.Roles
{
    /// <summary>
    /// Represents role record of a user.
    /// TODO: Add a unique index for UserId, RoleId
    /// </summary>
    public class UserRole : Entity<long>, ICreationAudited
    {
        /// <summary>
        /// User.
        /// </summary>
        public virtual AbpUser User { get; set; }

        /// <summary>
        /// Role.
        /// </summary>
        public virtual AbpRole Role { get; set; }

        /// <summary>
        /// Creation date of this entity.
        /// </summary>
        public virtual DateTime CreationTime { get; set; }

        /// <summary>
        /// Creator user of this entity.
        /// </summary>
        public virtual int? CreatorUserId { get; set; }
    }
}