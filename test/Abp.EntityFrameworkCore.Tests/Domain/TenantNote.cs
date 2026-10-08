using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain;

public class TenantNote : Entity, ISoftDelete, IMayHaveTenant
{
    public string Title { get; set; }

    public bool IsDeleted { get; set; }

    public int? TenantId { get; set; }
}
