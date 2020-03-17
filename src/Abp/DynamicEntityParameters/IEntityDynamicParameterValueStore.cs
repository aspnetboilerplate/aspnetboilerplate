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

        List<EntityDynamicParameterValue> GetValues(int entityDynamicParameterId, string entityId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(int entityDynamicParameterId, string entityId);

        List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityId);

        List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityId, int dynamicParameterId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityId, int dynamicParameterId);

        void CleanValues(int entityDynamicParameterId, string entityId);

        Task CleanValuesAsync(int entityDynamicParameterId, string entityId);
    }
}
