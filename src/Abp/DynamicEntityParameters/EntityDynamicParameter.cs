using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    [Table("AbpEntityDynamicParameters")]
    public class EntityDynamicParameter : Entity, IMayHaveTenant
    {
        public string EntityFullName { get; set; }

        [Required]
        public int DynamicParameterId { get; set; }

        [ForeignKey("DynamicParameterId")]
        public virtual DynamicParameter DynamicParameter { get; set; }

        public int? TenantId { get; set; }
    }
}
