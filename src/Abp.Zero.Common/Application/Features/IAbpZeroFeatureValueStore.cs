using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public interface IAbpZeroFeatureValueStore : IFeatureValueStore
    {
        Task<string> GetValueOrNullAsync(long tenantId, string featureName);
        string GetValueOrNull(long tenantId, string featureName);
        Task<string> GetEditionValueOrNullAsync(int editionId, string featureName);
        string GetEditionValueOrNull(int editionId, string featureName);
        Task SetEditionFeatureValueAsync(int editionId, string featureName, string value);
        void SetEditionFeatureValue(int editionId, string featureName, string value);
    }
}