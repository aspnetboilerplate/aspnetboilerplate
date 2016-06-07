using System.Collections.Generic;
using Abp.Domain.Services;

namespace AbpAspNetCoreDemo.Core.Domain
{
    public class ProductService : DomainService, IProductService
    {
        public List<Product> GetAll()
        {
            return new List<Product>
            {
                new Product("Acme Monitor 23 Inch.", 999.0f) {Id = 1},
                new Product("Acme Wireless Keyboard.", 79.9f) {Id = 2}
            };
        }
    }
}