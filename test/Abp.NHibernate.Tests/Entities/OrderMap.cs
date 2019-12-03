using Abp.NHibernate.EntityMappings;

namespace Abp.NHibernate.Tests.Entities
{
    public class OrderMap : EntityMap<Order>
    {
        public OrderMap() : base("Orders")
        {
            Id(x => x.Id);
            Map(x => x.TotalPrice);
            Map(x => x.CreationTime);

            HasMany(x => x.Items).LazyLoad();
        }
    }

    public class OrderDetailMap : EntityMap<OrderDetail>
    {
        public OrderDetailMap() : base("OrderDetails")
        {
            Id(x => x.Id);
            Map(x => x.ItemName);
            Map(x => x.Price);
            Map(x => x.Count);
            Map(x => x.TotalPrice);
            Map(x => x.CreationTime);
            References(x => x.Order).Column("OrderId");
        }
    }
}