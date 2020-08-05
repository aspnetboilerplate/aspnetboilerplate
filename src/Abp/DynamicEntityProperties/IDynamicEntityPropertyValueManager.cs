using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyValueManager
    {
        DynamicEntityPropertyValue Get(int id);

        Task<DynamicEntityPropertyValue> GetAsync(int id);

        void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue);

        void Delete(int id);

        Task DeleteAsync(int id);

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
