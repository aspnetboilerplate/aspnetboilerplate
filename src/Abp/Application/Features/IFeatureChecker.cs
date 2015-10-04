using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public interface IFeatureChecker
    {
        Task<bool> IsEnabledAsync(string name);

        Task<bool> IsEnabledAsync(int tenantId, string name);

        Task<string> GetValueAsync(string name);

        Task<string> GetValueAsync(int tenantId, string name);
    }
}