using Abp.Domain.Entities.Auditing;

namespace Abp.Zero.SampleApp.EntityHistory.Nhibernate
{
    public class NhEmployee : FullAuditedEntity
    {
        public virtual string FullName { get; set; }

        public virtual NhDepartment Department { get; set; }
    }

    public enum NhDepartment
    {
        Sales = 1,
        Marketing = 2,
        Development = 3,
        HumanResources = 4
    }
}