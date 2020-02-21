using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IEntityDynamicParameterValueManager
    {
        EntityDynamicParameterValue Get(int id);

        Task<EntityDynamicParameterValue> GetAsync(int id);

        EntityDynamicParameterValue Get(string entityId);

        Task<EntityDynamicParameterValue> GetAsync(string entityId);

        void Add(EntityDynamicParameterValue dynamicParameterValue);

        Task AddAsync(EntityDynamicParameterValue dynamicParameterValue);

        void Update(EntityDynamicParameterValue dynamicParameterValue);

        Task UpdateAsync(EntityDynamicParameterValue dynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);

        List<string> GetAllPossibleValueOfDynamicParameter(int dynamicParameterId);

        Task<List<string>> GetAllPossibleValueOfDynamicParameterAsync(int dynamicParameterId);
    }
}
