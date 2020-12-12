using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyValueManager
    {
        DynamicEntityPropertyValue Get(long id);

        Task<DynamicEntityPropertyValue> GetAsync(long id);

        void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        void Delete(long id);

        Task DeleteAsync(long id);

        List<DynamicEntityPropertyValue> GetValues(int dynamicEntityPropertyId, string entityId);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(int dynamicEntityPropertyId, string entityId);

        List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId);

        List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, int dynamicPropertyId);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId, int dynamicPropertyId);

        List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, string propertyName);

        Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId, string propertyName);

        void CleanValues(int dynamicEntityPropertyId, string entityId);

        Task CleanValuesAsync(int dynamicEntityPropertyId, string entityId);
    }
}
