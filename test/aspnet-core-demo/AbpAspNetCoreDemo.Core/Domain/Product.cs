using Abp.Domain.Entities;

namespace AbpAspNetCoreDemo.Core.Domain
{
    public class Product : Entity
    {
        public string Name { get; set; }

        public float Price { get; set; }

        public Product()
        {
            
        }

        public Product(string name, float price = 0.0f)
        {
            Name = name;
            Price = price;
        }
    }
}