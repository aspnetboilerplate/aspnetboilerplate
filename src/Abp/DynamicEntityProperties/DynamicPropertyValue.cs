using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityProperties
{
    [Table("AbpDynamicPropertyValues")]
    public class DynamicPropertyValue : Entity, IMayHaveTenant
    {
        /// <summary>
        /// Value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        public int? TenantId { get; set; }

        public int DynamicPropertyId { get; set; }

        [ForeignKey("DynamicPropertyId")]
        public virtual DynamicProperty DynamicProperty { get; set; }

        public DynamicPropertyValue()
        {
        }

        public DynamicPropertyValue(DynamicProperty dynamicProperty, string value, int? tenantId)
        {
            Value = value;
            TenantId = tenantId;
            DynamicPropertyId = dynamicProperty.Id;
        }
    }
}
