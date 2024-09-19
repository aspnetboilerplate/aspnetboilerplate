using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core.EntityHistory;

public class Employee : FullAuditedEntity
{
    public string FullName { get; set; }

    public Department Department { get; set; }
}

public enum Department
{
    Sales = 1,
    Marketing = 2,
    Development = 3,
    HumanResources = 4
}