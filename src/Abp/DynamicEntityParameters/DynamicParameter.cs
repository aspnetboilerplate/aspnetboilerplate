using System.Collections.Generic;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    public class DynamicParameter : Entity
    {
        public string ParameterName { get; set; }

        public string InputType { get; set; }

        public string Permission { get; set; }

        public virtual ICollection<DynamicParameterValues> DynamicParameterValues { get; set; }
    }
}
