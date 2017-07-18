using Abp.Dapper.Tests.Entities;

using DapperExtensions.Mapper;

namespace Abp.Dapper.Tests.Mappings
{
    public sealed class ProductDetailMap : ClassMapper<ProductDetail>
    {
        public ProductDetailMap()
        {
            Table("ProductDetails");
            AutoMap();
        }
    }
}
