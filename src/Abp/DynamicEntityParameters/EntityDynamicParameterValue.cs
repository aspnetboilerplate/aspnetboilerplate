using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    [Table("AbpEntityDynamicParameterValues")]
    public class EntityDynamicParameterValue : Entity, IMayHaveTenant
    {
        /// <summary>
        /// Value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public virtual string Value { get; set; }

        public string EntityRowId { get; set; }

        public int EntityDynamicParameterId { get; set; }

        public virtual EntityDynamicParameter EntityDynamicParameter { get; set; }

        public int? TenantId { get; set; }
    }
}
