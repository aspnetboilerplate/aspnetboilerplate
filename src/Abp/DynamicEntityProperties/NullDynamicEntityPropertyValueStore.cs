using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public class NullDynamicEntityPropertyValueStore : IDynamicEntityPropertyValueStore
    {
        public static NullDynamicEntityPropertyValueStore Instance = new NullDynamicEntityPropertyValueStore();

        public DynamicEntityPropertyValue Get(int id)
        {
            return default;
        }

        public Task<DynamicEntityPropertyValue> GetAsync(int id)
        {
            return Task.FromResult<DynamicEntityPropertyValue>(default);
        }

        public void Add(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
        }

        public Task AddAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicEntityPropertyValue dynamicEntityPropertyValue)
        {
        }

        public Task UpdateAsync(DynamicEntityPropertyValue dynamicEntityPropertyValue)
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

        public List<DynamicEntityPropertyValue> GetValues(int dynamicEntityPropertyId, string entityId)
        {
            return new List<DynamicEntityPropertyValue>();
        }

        public Task<List<DynamicEntityPropertyValue>> GetValuesAsync(int dynamicEntityPropertyId, string entityId)
        {
            return Task.FromResult(new List<DynamicEntityPropertyValue>());
        }

        public List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId)
        {
            return new List<DynamicEntityPropertyValue>();
        }

        public Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId)
        {
            return Task.FromResult(new List<DynamicEntityPropertyValue>());
        }

        public List<DynamicEntityPropertyValue> GetValues(string entityFullName, string entityId, int dynamicPropertyId)
        {
            return new List<DynamicEntityPropertyValue>();
        }

        public Task<List<DynamicEntityPropertyValue>> GetValuesAsync(string entityFullName, string entityId, int dynamicPropertyId)
        {
            return Task.FromResult(new List<DynamicEntityPropertyValue>());
        }

        public void CleanValues(int dynamicEntityPropertyId, string entityId)
        {
        }

        public Task CleanValuesAsync(int dynamicEntityPropertyId, string entityId)
        {
            return Task.CompletedTask;
        }
    }
}
