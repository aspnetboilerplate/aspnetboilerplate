using Abp.Domain.Entities;

namespace Abp.ZeroCore.SampleApp.Core.Shop
{
    public class OrderTranslation : Entity, IEntityTranslation<Order>
    {
        public virtual string Name { get; set; }

        public virtual Order Core { get; set; }

        public virtual int CoreId { get; set; }

        public virtual string Language { get; set; }
    }
}