using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH
{
    public abstract class Person : Entity
    {
        public virtual string Name { get; set; }
    }

    public abstract class PersonWithIdCard : Person
    {
        public virtual string IdCard { get; set; }
    }

    public abstract class PersonWithIdCardAndAddress : PersonWithIdCard
    {
        public virtual string Address { get; set; }
    }
}