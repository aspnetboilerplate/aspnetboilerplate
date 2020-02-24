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
        public virtual string Value { get; set; }

        public int? TenantId { get; set; }

        public int DynamicEntityParameterId { get; set; }

        [ForeignKey("DynamicEntityParameterId")]
        public virtual DynamicParameter DynamicEntityParameter { get; set; }
    }
}
