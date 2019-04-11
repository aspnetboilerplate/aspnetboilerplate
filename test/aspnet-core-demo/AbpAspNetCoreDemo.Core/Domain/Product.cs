using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;

namespace AbpAspNetCoreDemo.Core.Domain
{
    [Table("AppProducts")]
    public class Product : Entity
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