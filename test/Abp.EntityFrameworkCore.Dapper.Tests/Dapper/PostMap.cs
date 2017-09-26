using Abp.EntityFrameworkCore.Dapper.Tests.Domain;

using DapperExtensions.Mapper;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Dapper
{
    public class PostMap : ClassMapper<Post>
    {
        public PostMap()
        {
            Table("Posts");
            Map(x => x.Id).Key(KeyType.Guid);
            Map(x => x.Blog).Ignore();
            Map(x => x.Comment).Ignore();
            AutoMap();
        }
    }
}
