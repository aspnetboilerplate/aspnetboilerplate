using Abp.NHibernate.EntityMappings;

namespace Abp.NHibernate.Tests
{
    public class PersonMap : EntityMap<Person>
    {
        public PersonMap()
            : base("People")
        {
            Map(x => x.Name);
        }
    }
}