using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityProperties
{
    [Table("AbpDynamicEntityProperties")]
    public class DynamicEntityProperty : Entity, IMayHaveTenant
    {
        /// <summary>
        /// Maximum length of the <see cref="EntityFullName"/> property.
        /// </summary>
        public const int MaxEntityFullName = 256;
        
        [StringLength(MaxEntityFullName)]
        public string EntityFullName { get; set; }

        [Required]
        public int DynamicPropertyId { get; set; }

        public int? TenantId { get; set; }
        
        [ForeignKey("DynamicPropertyId")]
        public virtual DynamicProperty DynamicProperty { get; set; }

    }
}
