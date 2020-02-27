using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    [Table("AbpDynamicParameters")]
    public class DynamicParameter : Entity, IMayHaveTenant
    {
        public string ParameterName { get; set; }

        public string InputType { get; set; }

        public string Permission { get; set; }

        public virtual ICollection<DynamicParameterValue> DynamicParameterValues { get; set; }

        public int? TenantId { get; set; }
    }
}
