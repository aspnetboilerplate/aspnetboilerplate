using System.Collections.Generic;
using System.Linq;
using Abp.MultiTenancy;

namespace Abp.AspNetCore.App.MultiTenancy;

public class TestTenantStore : ITenantStore
{
    private readonly List<TenantInfo> _tenants = new List<TenantInfo>
        {
            new TenantInfo(1, "Default"),
            new TenantInfo(42, "acme"),
            new TenantInfo(43, "vlsft")
        };

    public TenantInfo Find(int tenantId)
    {
        return _tenants.FirstOrDefault(t => t.Id == tenantId);
    }

    public TenantInfo Find(string tenancyName)
    {
        return _tenants.FirstOrDefault(t => t.TenancyName.ToLower() == tenancyName.ToLower());
    }
}