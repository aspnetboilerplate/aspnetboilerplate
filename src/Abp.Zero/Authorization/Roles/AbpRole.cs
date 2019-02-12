using System.ComponentModel.DataAnnotations;
using Abp.Authorization.Users;
using Abp.Domain.Entities.Auditing;
using Microsoft.AspNet.Identity;

namespace Abp.Authorization.Roles
{
    /// <summary>
    /// Represents a role in an application. A role is used to group permissions.
    /// </summary>
    /// <remarks> 
    /// Application should use permissions to check if user is granted to perform an operation.
    /// Checking 'if a user has a role' is not possible until the role is static (<see cref="AbpRoleBase.IsStatic"/>).
    /// Static roles can be used in the code and can not be deleted by users.
    /// Non-static (dynamic) roles can be added/removed by users and we can not know their name while coding.
    /// A user can have multiple roles. Thus, user will have all permissions of all assigned roles.
    /// </remarks>
    public abstract class AbpRole<TUser> : AbpRoleBase, IRole<int>, IFullAudited<TUser>
        where TUser : AbpUser<TUser>
    {
        /// <summary>
        /// Unique name of this role.
        /// </summary>
        [Required]
        [StringLength(MaxNameLength)]
        public virtual string NormalizedName { get; set; }

        public virtual TUser DeleterUser { get; set; }

        public virtual TUser CreatorUser { get; set; }

        public virtual TUser LastModifierUser { get; set; }

        protected AbpRole()
        {
            SetNormalizedName();
        }

        /// <summary>
        /// Creates a new <see cref="AbpRole{TUser}"/> object.
        /// </summary>
        /// <param name="tenantId">TenantId or null (if this is not a tenant-level role)</param>
        /// <param name="displayName">Display name of the role</param>
        protected AbpRole(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
            SetNormalizedName();
        }

        /// <summary>
        /// Creates a new <see cref="AbpRole{TUser}"/> object.
        /// </summary>
        /// <param name="tenantId">TenantId or null (if this is not a tenant-level role)</param>
        /// <param name="name">Unique role name</param>
        /// <param name="displayName">Display name of the role</param>
        protected AbpRole(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
            SetNormalizedName();
        }

        public virtual void SetNormalizedName()
        {
            NormalizedName = Name.ToUpperInvariant();
        }
    }
}