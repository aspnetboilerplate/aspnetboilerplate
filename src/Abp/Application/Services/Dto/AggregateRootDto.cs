using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using Abp.Domain.Entities;
using Abp.Events.Bus;

namespace Abp.Application.Services.Dto
{
    public abstract class AggregateRootDto : AggregateRootDto<int>
    {

    }

    public abstract class AggregateRootDto<TPrimaryKey> : EntityDto<TPrimaryKey>, IGeneratesDomainEvents
    {
        [NotMapped]
        public virtual ICollection<IEventData> DomainEvents { get; }

        protected AggregateRootDto()
        {
            DomainEvents = new Collection<IEventData>();
        }
    }
}