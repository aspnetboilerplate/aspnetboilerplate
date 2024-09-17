using Abp.Domain.Entities;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    public class NhAdvertisement : Entity
    {
        public virtual string Banner { get; set; }
    }
}
