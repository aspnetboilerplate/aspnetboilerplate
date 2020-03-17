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

        public List<EntityDynamicParameterValue> GetValues(int entityDynamicParameterId, string entityId)
        {
            return new List<EntityDynamicParameterValue>();
        }

        public Task<List<EntityDynamicParameterValue>> GetValuesAsync(int entityDynamicParameterId, string entityId)
        {
            return Task.FromResult(new List<EntityDynamicParameterValue>());
        }

        public List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityId)
        {
            return new List<EntityDynamicParameterValue>();
        }

        public Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityId)
        {
            return Task.FromResult(new List<EntityDynamicParameterValue>());
        }

        public List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityId, int dynamicParameterId)
        {
            return new List<EntityDynamicParameterValue>();
        }

        public Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityId, int dynamicParameterId)
        {
            return Task.FromResult(new List<EntityDynamicParameterValue>());
        }

        public void CleanValues(int entityDynamicParameterId, string entityId)
        {
        }

        public Task CleanValuesAsync(int entityDynamicParameterId, string entityId)
        {
            return Task.CompletedTask;
        }
    }
}
