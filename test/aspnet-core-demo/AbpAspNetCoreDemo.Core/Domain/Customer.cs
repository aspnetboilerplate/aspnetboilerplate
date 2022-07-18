using Abp.Domain.Entities;

namespace AbpAspNetCoreDemo.Core.Domain
{
    public class Customer : Entity, IMustHaveTenant
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public int TenantId { get; set; }
    }
}