using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicParameterValueStore
    {
        DynamicParameterValue Get(int id);

        Task<DynamicParameterValue> GetAsync(int id);

        List<DynamicParameterValue> GetAllValuesOfDynamicParameter(int dynamicParameterId);

        Task<List<DynamicParameterValue>> GetAllValuesOfDynamicParameterAsync(int dynamicParameterId);

        void Add(DynamicParameterValue dynamicParameterValue);

        Task AddAsync(DynamicParameterValue dynamicParameterValue);

        void Update(DynamicParameterValue dynamicParameterValue);

        Task UpdateAsync(DynamicParameterValue dynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);

        void CleanValues(int dynamicParameterId);

        Task CleanValuesAsync(int dynamicParameterId);
    }
}
