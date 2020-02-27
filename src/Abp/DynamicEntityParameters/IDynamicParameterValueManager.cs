using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicParameterValueManager
    {
        DynamicParameterValue Get(int id);

        Task<DynamicParameterValue> GetAsync(int id);

        List<DynamicParameterValue> GetAll(int dynamicParameterId);

        Task<List<DynamicParameterValue>> GetAllAsync(int dynamicParameterId);

        void Add(DynamicParameterValue dynamicParameterValue);

        Task AddAsync(DynamicParameterValue dynamicParameterValue);

        void Update(DynamicParameterValue dynamicParameterValue);

        Task UpdateAsync(DynamicParameterValue dynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
