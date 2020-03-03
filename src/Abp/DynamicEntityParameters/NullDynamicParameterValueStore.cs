using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public class NullDynamicParameterValueStore : IDynamicParameterValueStore
    {
        public static NullDynamicParameterValueStore Instance = new NullDynamicParameterValueStore();

        public DynamicParameterValue Get(int id)
        {
            return default;
        }

        public Task<DynamicParameterValue> GetAsync(int id)
        {
            return Task.FromResult<DynamicParameterValue>(default);
        }

        public List<DynamicParameterValue> GetAllValuesOfDynamicParameter(int dynamicParameterId)
        {
            return new List<DynamicParameterValue>();
        }

        public Task<List<DynamicParameterValue>> GetAllValuesOfDynamicParameterAsync(int dynamicParameterId)
        {
            return Task.FromResult(new List<DynamicParameterValue>());
        }

        public void Add(DynamicParameterValue dynamicParameterValue)
        {
        }

        public Task AddAsync(DynamicParameterValue dynamicParameterValue)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicParameterValue dynamicParameterValue)
        {
        }

        public Task UpdateAsync(DynamicParameterValue dynamicParameterValue)
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

        public void CleanValues(int dynamicParameterId)
        {
        }

        public Task CleanValuesAsync(int dynamicParameterId)
        {
            return Task.CompletedTask;
        }
    }
}
