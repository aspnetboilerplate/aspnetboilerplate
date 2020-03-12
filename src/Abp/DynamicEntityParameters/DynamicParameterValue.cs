using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    [Table("AbpDynamicParameterValues")]
    public class DynamicParameterValue : Entity, IMayHaveTenant
    {
        /// <summary>
        /// Value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        public int? TenantId { get; set; }

        public int DynamicParameterId { get; set; }

        [ForeignKey("DynamicParameterId")]
        public virtual DynamicParameter DynamicParameter { get; set; }

        public DynamicParameterValue()
        {
        }

        public DynamicParameterValue(DynamicParameter dynamicParameter, string value, int? tenantId)
        {
            Value = value;
            TenantId = tenantId;
            DynamicParameterId = dynamicParameter.Id;
        }
    }
}
