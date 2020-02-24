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

        public void Add(EntityDynamicParameterValue dynamicParameterValue)
        {
        }

        public Task AddAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            return Task.CompletedTask;
        }

        public void Update(EntityDynamicParameterValue dynamicParameterValue)
        {
        }

        public Task UpdateAsync(EntityDynamicParameterValue dynamicParameterValue)
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

        public List<EntityDynamicParameterValue> GetValues(string entityRowId, int parameterId)
        {
            return new List<EntityDynamicParameterValue>();
        }

        public Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityRowId, int parameterId)
        {
            return Task.FromResult(new List<EntityDynamicParameterValue>());
        }

        public void CleanValues(string entityRowId, int parameterId)
        {
        }

        public Task CleanValuesAsync(string entityRowId, int parameterId)
        {
            return Task.CompletedTask;
        }
    }
}
