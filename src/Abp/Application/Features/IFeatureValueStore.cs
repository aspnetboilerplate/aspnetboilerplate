using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public interface IFeatureValueStore
    {
        Task<string> GetValueOrNullAsync(int tenantId, Feature feature);
    }
}