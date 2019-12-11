using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH
{
    public abstract class Person : Entity
    {
        public string Name { get; set; }
    }

    public abstract class PersonWithIdCard : Person
    {
        public string IdCard { get; set; }
    }

    public abstract class PersonWithIdCardAndAddress : PersonWithIdCard
    {
        public string Address { get; set; }
    }
}