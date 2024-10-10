using Abp.EntityFrameworkCore.Dapper.Tests.Domain;

using DapperExtensions.Mapper;

namespace Abp.EntityFrameworkCore.Dapper.Tests.Dapper;

public sealed class CommentMap : ClassMapper<Comment>
{
    public CommentMap()
    {
        Table("Comments");
        Map(x => x.Id).Key(KeyType.Identity);
        AutoMap();
    }
}