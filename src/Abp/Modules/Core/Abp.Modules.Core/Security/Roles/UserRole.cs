using System;
using System.ComponentModel.DataAnnotations.Schema;
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
        [ForeignKey("UserId")]
        public virtual AbpUser User { get; set; }

        public virtual int UserId { get; set; } //Remove later

        /// <summary>
        /// Role.
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual AbpRole Role { get; set; }
        public virtual int RoleId { get; set; } //Remove later

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