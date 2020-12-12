using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicPropertyValueStore
    {
        DynamicPropertyValue Get(long id);

        Task<DynamicPropertyValue> GetAsync(long id);

        List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(int dynamicPropertyId);

        Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(int dynamicPropertyId);

        void Add(DynamicPropertyValue dynamicPropertyValue);

        Task AddAsync(DynamicPropertyValue dynamicPropertyValue);

        void Update(DynamicPropertyValue dynamicPropertyValue);

        Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue);

        void Delete(long id);

        Task DeleteAsync(long id);

        void CleanValues(int dynamicPropertyId);

        Task CleanValuesAsync(int dynamicPropertyId);
    }
}
