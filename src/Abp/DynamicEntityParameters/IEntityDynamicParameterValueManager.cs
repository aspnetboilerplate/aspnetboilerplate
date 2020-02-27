using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IEntityDynamicParameterValueManager
    {
        EntityDynamicParameterValue Get(int id);

        Task<EntityDynamicParameterValue> GetAsync(int id);

        void Add(EntityDynamicParameterValue entityDynamicParameterValue);

        Task AddAsync(EntityDynamicParameterValue entityDynamicParameterValue);

        void Update(EntityDynamicParameterValue entityDynamicParameterValue);

        Task UpdateAsync(EntityDynamicParameterValue entityDynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);

        List<EntityDynamicParameterValue> GetValues(string entityRowId, int parameterId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityRowId, int parameterId);

        void CleanValues(string entityRowId, int parameterId);

        Task CleanValuesAsync(string entityRowId, int parameterId);
    }
}
