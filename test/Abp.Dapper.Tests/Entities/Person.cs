using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using System.ComponentModel.DataAnnotations.Schema;

namespace Abp.Dapper.Tests.Entities
{
    [Table("Person")]
    public class Person : Entity, IMustHaveTenant
    {
        
        protected Person()
        {
        }

        public Person(string name) : this()
        {
            Name = name;
        }

        public virtual string Name { get; set; }

        public int TenantId { get; set; }
    }
}
