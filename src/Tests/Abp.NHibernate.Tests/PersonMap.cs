using Adorable.NHibernate.EntityMappings;

namespace Adorable.NHibernate.Tests
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