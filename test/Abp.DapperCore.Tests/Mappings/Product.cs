using Abp.DapperCore.Tests.Entities;

using DapperExtensions.Mapper;

namespace Abp.DapperCore.Tests.Mappings
{
    public sealed class ProductMap : ClassMapper<Product>
    {
        public ProductMap()
        {
            Table("Products");
            AutoMap();
        }
    }
}
