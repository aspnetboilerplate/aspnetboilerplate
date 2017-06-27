using Abp.Dapper.Tests.Entities;

using DapperExtensions.Mapper;

namespace Abp.Dapper.Tests.Mappings
{
    public sealed class ProductMap : ClassMapper<Product>
    {
        public ProductMap()
        {
            Table("Products");
            Map(x => x.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }
}
