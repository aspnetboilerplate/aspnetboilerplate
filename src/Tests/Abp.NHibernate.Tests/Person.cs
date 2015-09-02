using Abp.Domain.Entities;

namespace Abp.NHibernate.Tests
{
    public class Person : Entity
    {
        public const int MaxNameLength = 64;

        public virtual string Name { get; set; }
    }
}