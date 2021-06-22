using System.Threading.Tasks;

namespace Abp.MultiTenancy
{
    public interface ITenantResolver
    {
        int? ResolveTenantId();
        
        Task<int?> ResolveTenantIdAsync();
    }
}
