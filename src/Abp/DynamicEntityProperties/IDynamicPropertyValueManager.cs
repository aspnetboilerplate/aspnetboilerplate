using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicPropertyValueManager
    {
        DynamicPropertyValue Get(int id);

        Task<DynamicPropertyValue> GetAsync(int id);

        List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(int dynamicPropertyId);

        Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(int dynamicPropertyId);

        void Add(DynamicPropertyValue dynamicPropertyValue);

        Task AddAsync(DynamicPropertyValue dynamicPropertyValue);

        void Update(DynamicPropertyValue dynamicPropertyValue);

        Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue);

        void Delete(int id);

        Task DeleteAsync(int id);

        void CleanValues(int dynamicPropertyId);

        Task CleanValuesAsync(int dynamicPropertyId);
    }
}
