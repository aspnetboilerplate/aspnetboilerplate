using Abp.NHibernate.EntityMappings;

namespace Abp.Dapper.NHibernate.Tests.Mappings.NHibernate
{
    public class PersonMap : EntityMap<Person>
    {
        public PersonMap() : base("Persons")
        {
            Map(x => x.Name);
        }
    }
}
