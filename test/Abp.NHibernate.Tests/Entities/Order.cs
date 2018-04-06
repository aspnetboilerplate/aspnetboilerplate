using System;
using System.Collections.Generic;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Timing;

namespace Abp.NHibernate.Tests.Entities
{
    public class Order : Entity, IHasCreationTime
    {
        public virtual decimal TotalPrice { get; set; }

        [DisableDateTimeNormalization]
        public virtual DateTime CreationTime { get; set; }

        public virtual ICollection<OrderDetail> Items { get; set; }
    }
}