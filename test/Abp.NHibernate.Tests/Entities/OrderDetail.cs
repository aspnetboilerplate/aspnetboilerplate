using System;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Abp.NHibernate.Tests.Entities
{
    [DisableDateTimeNormalization]
    public class OrderDetail : Entity, IHasCreationTime
    {
        public virtual Order Order { get; set; }

        public virtual string ItemName { get; set; }

        public virtual decimal Price { get; set; }

        public virtual int Count { get; set; }

        public virtual decimal TotalPrice { get; set; }

        public virtual DateTime CreationTime { get; set; }
    }
}