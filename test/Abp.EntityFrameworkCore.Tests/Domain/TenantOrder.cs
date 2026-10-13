using Abp.Domain.Entities;

namespace Abp.EntityFrameworkCore.Tests.Domain;

public class TenantOrder : Entity, IMustHaveTenant
{
    public string Number { get; set; }

    public int TenantId { get; set; }
}
