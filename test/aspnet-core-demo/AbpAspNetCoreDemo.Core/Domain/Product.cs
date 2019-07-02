using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities.Auditing;

namespace AbpAspNetCoreDemo.Core.Domain
{
    [Table("AppProducts")]
    public class Product : FullAuditedEntity
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        public float? Price { get; set; }

        public Product()
        {
            
        }

        public Product(string name, float? price = null)
        {
            Name = name;
            Price = price;
        }
    }
}