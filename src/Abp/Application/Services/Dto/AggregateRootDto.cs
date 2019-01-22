using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Events.Bus;

namespace Abp.Application.Services.Dto
{

    public class AggregateRootDto : AggregateRootDto<int>, IAggregateRoot
    {

    }

    public class AggregateRootDto<TPrimaryKey> : Entity<TPrimaryKey>, IAggregateRoot<TPrimaryKey>
    {
        [NotMapped]
        public virtual ICollection<IEventData> DomainEvents { get; }

        public AggregateRootDto()
        {
            DomainEvents = new Collection<IEventData>();
        }
    }

}