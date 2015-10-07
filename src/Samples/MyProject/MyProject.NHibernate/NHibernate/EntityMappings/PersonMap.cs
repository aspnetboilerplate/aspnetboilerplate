using Abp.NHibernate.EntityMappings;
using MyProject.People;

namespace MyProject.NHibernate.EntityMappings
{

    public class PersonMap : EntityMap<Person>
    {
        public PersonMap()
            : base("StsPeople")
        {
            Map(x => x.Name);
        }
    }
}
