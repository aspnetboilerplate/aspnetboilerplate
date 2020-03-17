using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    [Table("AbpEntityDynamicParameterValues")]
    public class EntityDynamicParameterValue : Entity, IMayHaveTenant
    {
        [Required(AllowEmptyStrings = false)]
        public string Value { get; set; }

        public string EntityId { get; set; }

        public int EntityDynamicParameterId { get; set; }

        public virtual EntityDynamicParameter EntityDynamicParameter { get; set; }

        public int? TenantId { get; set; }

        public EntityDynamicParameterValue()
        {

        }

        public EntityDynamicParameterValue(EntityDynamicParameter entityDynamicParameter, string entityId, string value, int? tenantId)
        {
            EntityDynamicParameterId = entityDynamicParameter.Id;
            EntityId = entityId;
            Value = value;
            TenantId = tenantId;
        }
    }
}
