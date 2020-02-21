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

        public EntityDynamicParameterValue Get(string entityId)
        {
            return default;
        }

        public Task<EntityDynamicParameterValue> GetAsync(string entityId)
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

        public List<string> GetAllPossibleValueOfDynamicParameter(int dynamicParameterId)
        {
            return new List<string>();
        }

        public Task<List<string>> GetAllPossibleValueOfDynamicParameterAsync(int dynamicParameterId)
        {
            return Task.FromResult(new List<string>());
        }
    }
}
