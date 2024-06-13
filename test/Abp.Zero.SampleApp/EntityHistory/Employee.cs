using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory
{
    public class Employee : FullAuditedEntity
    {
        public virtual string FullName { get; set; }

        public virtual Department Department { get; set; }
    }

    public enum Department
    {
        Sales = 1,
        Marketing = 2,
        Development = 3,
        HumanResources = 4
    }
}