using Abp.Domain.Entities;

namespace MySpaProject.People
{
    public class Person : Entity
    {
        public virtual string Name { get; set; }
    }
}