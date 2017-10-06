using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain
{
    public class Ticket : Entity, IPassivable, IMustHaveTenant
    {
        public virtual string EmailAddress { get; set; }

        public virtual string Message { get; set; }

        public virtual bool IsActive { get; set; }

        public virtual int TenantId { get; set; }

        public Ticket()
        {
            IsActive = true;
        }
    }
}