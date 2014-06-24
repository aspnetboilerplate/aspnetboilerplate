using System;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("UserId")]
        public virtual AbpUser User { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual int UserId { get; set; } //Needed for EntityFramework. Try to remove if possible!

        /// <summary>
        /// Role.
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual AbpRole Role { get; set; }

        /// <summary>
        /// Role Id.
        /// </summary>
        public virtual int RoleId { get; set; } //Needed for EntityFramework. Try to remove if possible!

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