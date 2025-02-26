using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization.Users;

namespace Abp.MultiTenancy;

public interface IAbpTenantManager<TTenant, TUser>
    where TTenant : AbpTenant<TUser>
    where TUser : AbpUserBase
{
    IQueryable<TTenant> Tenants { get; }

    Task CreateAsync(TTenant tenant);
    void Create(TTenant tenant);
    Task UpdateAsync(TTenant tenant);
    void Update(TTenant tenant);
    Task<TTenant> FindByIdAsync(int id);
    TTenant FindById(int id);
    Task<TTenant> GetByIdAsync(int id);
    TTenant GetById(int id);
    Task<TTenant> FindByTenancyNameAsync(string tenancyName);
    TTenant FindByTenancyName(string tenancyName);
    Task DeleteAsync(TTenant tenant);
    void Delete(TTenant tenant);
    Task<string> GetFeatureValueOrNullAsync(int tenantId, string featureName);
    string GetFeatureValueOrNull(int tenantId, string featureName);
    Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int tenantId);
    IReadOnlyList<NameValue> GetFeatureValues(int tenantId);
    Task SetFeatureValuesAsync(int tenantId, params NameValue[] values);
    void SetFeatureValues(int tenantId, params NameValue[] values);
    Task SetFeatureValueAsync(int tenantId, string featureName, string value);
    void SetFeatureValue(int tenantId, string featureName, string value);
    Task ResetAllFeaturesAsync(int tenantId);
    void ResetAllFeatures(int tenantId);
}