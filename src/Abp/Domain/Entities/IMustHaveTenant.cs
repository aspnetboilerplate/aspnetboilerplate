using System;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// Implement this interface for an entity which must have TenantId.
    /// </summary>
    public interface IMustHaveTenant
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        Guid TenantId { get; set; }
    }
}