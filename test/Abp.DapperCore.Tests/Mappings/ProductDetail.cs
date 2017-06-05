using Abp.DapperCore.Tests.Entities;

using DapperExtensions.Mapper;

namespace Abp.DapperCore.Tests.Mappings
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
