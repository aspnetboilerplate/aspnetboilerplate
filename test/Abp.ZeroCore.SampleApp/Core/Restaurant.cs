using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;

namespace Abp.ZeroCore.SampleApp.Core;

public class Restaurant : FullAuditedEntity, IMayHaveTenant
{
    public string Name { get; set; }

    public string Cuisine { get; set; }

    public int? TenantId { get; set; }
}