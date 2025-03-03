using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Runtime.Caching;

namespace Abp.Application.Editions;

public interface IAbpEditionManager
{
    IQueryable<Edition> Editions { get; }

    ICacheManager CacheManager { get; set; }
    IFeatureManager FeatureManager { get; set; }

    Task<IQueryable<Edition>> GetEditionsAsync();
    Task<string> GetFeatureValueOrNullAsync(int editionId, string featureName);
    string GetFeatureValueOrNull(int editionId, string featureName);
    Task SetFeatureValueAsync(int editionId, string featureName, string value);
    void SetFeatureValue(int editionId, string featureName, string value);
    Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int editionId);
    IReadOnlyList<NameValue> GetFeatureValues(int editionId);
    Task SetFeatureValuesAsync(int editionId, params NameValue[] values);
    void SetFeatureValues(int editionId, params NameValue[] values);
    Task CreateAsync(Edition edition);
    void Create(Edition edition);
    Task<Edition> FindByNameAsync(string name);
    Edition FindByName(string name);
    Task<Edition> FindByIdAsync(int id);
    Edition FindById(int id);
    Task<Edition> GetByIdAsync(int id);
    Edition GetById(int id);
    Task DeleteAsync(Edition edition);
    void Delete(Edition edition);
}