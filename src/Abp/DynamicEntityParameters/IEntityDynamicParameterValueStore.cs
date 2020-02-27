using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IEntityDynamicParameterValueStore
    {
        EntityDynamicParameterValue Get(int id);

        Task<EntityDynamicParameterValue> GetAsync(int id);

        void Add(EntityDynamicParameterValue entityDynamicParameterValue);

        Task AddAsync(EntityDynamicParameterValue entityDynamicParameterValue);

        void Update(EntityDynamicParameterValue entityDynamicParameterValue);

        Task UpdateAsync(EntityDynamicParameterValue entityDynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);

        List<EntityDynamicParameterValue> GetValues(string entityRowId, int entityDynamicParameterId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityRowId, int entityDynamicParameterId);

        void CleanValues(string entityRowId, int entityDynamicParameterId);

        Task CleanValuesAsync(string entityRowId, int entityDynamicParameterId);
    }
}
