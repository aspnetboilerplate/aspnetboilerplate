using DapperExtensions.Mapper;

namespace Abp.Dapper.NHibernate.Tests.Nhibernate
{
    public class PersonMap : ClassMapper<Person>
    {
        public PersonMap()
        {
            Table("Persons");
            AutoMap();
        }
    }
}
