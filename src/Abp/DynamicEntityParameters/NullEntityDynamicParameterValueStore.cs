using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public class NullEntityDynamicParameterValueStore : IEntityDynamicParameterValueStore
    {
        public static NullEntityDynamicParameterValueStore Instance = new NullEntityDynamicParameterValueStore();

        public EntityDynamicParameterValue Get(int id)
        {
            return default;
        }

        public Task<EntityDynamicParameterValue> GetAsync(int id)
        {
            return Task.FromResult<EntityDynamicParameterValue>(default);
        }

        public void Add(EntityDynamicParameterValue entityDynamicParameterValue)
        {
        }

        public Task AddAsync(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            return Task.CompletedTask;
        }

        public void Update(EntityDynamicParameterValue entityDynamicParameterValue)
        {
        }

        public Task UpdateAsync(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            return Task.CompletedTask;
        }

        public void Delete(int id)
        {
        }

        public Task DeleteAsync(int id)
        {
            return Task.CompletedTask;
        }

        public List<EntityDynamicParameterValue> GetValues(int entityDynamicParameterId, string entityRowId)
        {
            return new List<EntityDynamicParameterValue>();
        }

        public Task<List<EntityDynamicParameterValue>> GetValuesAsync(int entityDynamicParameterId, string entityRowId)
        {
            return Task.FromResult(new List<EntityDynamicParameterValue>());
        }

        public List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityRowId)
        {
            return new List<EntityDynamicParameterValue>();
        }

        public Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityRowId)
        {
            return Task.FromResult(new List<EntityDynamicParameterValue>());
        }

        public void CleanValues(int entityDynamicParameterId, string entityRowId)
        {
        }

        public Task CleanValuesAsync(int entityDynamicParameterId, string entityRowId)
        {
            return Task.CompletedTask;
        }
    }
}
