using Abp.Dapper.Tests.Entities;

using DapperExtensions.Mapper;

namespace Abp.Dapper.Tests.Mappings
{
    public sealed class PersonMap : ClassMapper<Person>
    {
        public PersonMap()
        {
            Table("Person");
            Map(x => x.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }
}
