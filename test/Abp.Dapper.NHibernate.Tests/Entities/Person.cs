using Abp.Domain.Entities;

namespace Abp.Dapper.NHibernate.Tests
{
    public class Person : Entity
    {
        protected Person()
        {
        }

        public Person(string name) : this()
        {
            Name = name;
        }

        public virtual string Name { get; set; }
    }
}
