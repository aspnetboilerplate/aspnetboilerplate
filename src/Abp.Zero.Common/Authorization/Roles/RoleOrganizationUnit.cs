using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Organizations;

namespace Abp.Authorization.Roles
{
    /// <summary>
    /// Represents membership of a User to an OU.
    /// </summary>
    [Table("AbpRoleOrganizationUnit")]
    public class RoleOrganizationUnit : CreationAuditedEntity<long>, IMayHaveTenant, ISoftDelete
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        public virtual int? TenantId { get; set; }

        /// <summary>
        /// Id of the Role.
        /// </summary>
        public virtual int RoleId { get; set; }

        /// <summary>
        /// Id of the <see cref="OrganizationUnit"/>.
        /// </summary>
        public virtual long OrganizationUnitId { get; set; }

        /// <summary>
        /// Specifies if the organization is soft deleted or not.
        /// </summary>
        public virtual bool IsDeleted { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOrganizationUnit"/> class.
        /// </summary>
        public RoleOrganizationUnit()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleOrganizationUnit"/> class.
        /// </summary>
        /// <param name="tenantId">TenantId</param>
        /// <param name="roleId">Id of the User.</param>
        /// <param name="organizationUnitId">Id of the <see cref="OrganizationUnit"/>.</param>
        public RoleOrganizationUnit(int? tenantId, int roleId, long organizationUnitId)
        {
            TenantId = tenantId;
            RoleId = roleId;
            OrganizationUnitId = organizationUnitId;
        }
    }
}
