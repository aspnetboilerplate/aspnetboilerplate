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
    public class UserRole : CreationAuditedEntity<long>
    {
        /// <summary>
        /// User.
        /// </summary>
        [ForeignKey("UserId")]
        public virtual AbpUser User { get; set; }

        /// <summary>
        /// User Id.
        /// </summary>
        public virtual long UserId { get; set; } //Needed for EntityFramework. Try to remove if possible!

        /// <summary>
        /// Role.
        /// </summary>
        [ForeignKey("RoleId")]
        public virtual AbpRole Role { get; set; }

        /// <summary>
        /// Role Id.
        /// </summary>
        public virtual int RoleId { get; set; } //Needed for EntityFramework. Try to remove if possible!
    }
}