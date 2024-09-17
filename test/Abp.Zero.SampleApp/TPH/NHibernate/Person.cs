using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.TPH.NHibernate
{
    public abstract class NhPerson : Entity
    {
        public virtual string Name { get; set; }
    }

    public abstract class NhPersonWithIdCard : NhPerson
    {
        public virtual string IdCard { get; set; }
    }

    public abstract class NhPersonWithIdCardAndAddress : NhPersonWithIdCard
    {
        public virtual string Address { get; set; }
    }
}