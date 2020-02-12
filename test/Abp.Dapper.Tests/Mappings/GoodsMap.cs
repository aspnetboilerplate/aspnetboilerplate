using Abp.Dapper.Tests.Entities;
using DapperExtensions.Mapper;

namespace Abp.Dapper.Tests.Mappings
{
    public sealed class GoodsMap : ClassMapper<Good>
    {
        public GoodsMap()
        {
            Table("Goods");
            Map(x => x.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }
}