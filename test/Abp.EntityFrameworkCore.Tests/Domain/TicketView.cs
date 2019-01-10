using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    public class TicketListItem : IPassivable, IMustHaveTenant, IEntity<int>
    {
        public int Id { get; set; }

        public virtual string EmailAddress { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int TenantId { get; set; }

        public bool IsTransient()
        {
            return Id <= 0;
        }
    }
}
