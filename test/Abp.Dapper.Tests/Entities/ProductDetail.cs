using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.Dapper.Tests.Entities
{
    [Table("ProductDetails")]
    public class ProductDetail : FullAuditedEntity, IMustHaveTenant
    {
        protected ProductDetail()
        {
        }

        public ProductDetail(string gender) : this()
        {
            Gender = gender;
        }

        [Required]
        public string Gender { get; set; }

        public int TenantId { get; set; }
    }
}
