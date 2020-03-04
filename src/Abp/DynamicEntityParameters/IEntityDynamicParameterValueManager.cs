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

        List<EntityDynamicParameterValue> GetValues(int entityDynamicParameterId, string entityRowId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(int entityDynamicParameterId, string entityRowId);

        List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityRowId);

        Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityRowId);

        void CleanValues(int entityDynamicParameterId, string entityRowId);

        Task CleanValuesAsync(int entityDynamicParameterId, string entityRowId);
    }
}
