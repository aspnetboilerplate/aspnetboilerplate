using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain;

public class TenantMemo : Entity, IMayHaveTenant
{
    public string Text { get; set; }

    public int? TenantId { get; set; }
}
