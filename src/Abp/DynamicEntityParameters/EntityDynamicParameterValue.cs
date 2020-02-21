using System.ComponentModel.DataAnnotations;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterValue : Entity
    {
        /// <summary>
        /// Value.
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        public virtual string Value { get; set; }

        public string EntityRowId { get; set; }

        public int EntityDynamicParameterId { get; set; }

        public virtual EntityDynamicParameter EntityDynamicParameter { get; set; }
    }
}
