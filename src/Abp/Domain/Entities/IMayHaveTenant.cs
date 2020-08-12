using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.Domain.Entities
{
    /// <summary>
    /// Implement this interface for an entity which may optionally have TenantId.
    /// </summary>
    public interface IMayHaveTenant
    {
        /// <summary>
        /// TenantId of this entity.
        /// </summary>
        [Column("tenant_id")]
        long? TenantId { get; set; }
    }
}