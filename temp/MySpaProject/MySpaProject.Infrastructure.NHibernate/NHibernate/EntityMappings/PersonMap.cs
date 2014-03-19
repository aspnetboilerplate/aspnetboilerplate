using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abp.Domain.Entities.Mapping;
using MySpaProject.People;

namespace MySpaProject.NHibernate.EntityMappings
{
    public class PersonMap : EntityMap<Person>
    {
        public PersonMap()
            : base("TsPeople")
        {
            Map(x => x.Name);
        }
    }
}
