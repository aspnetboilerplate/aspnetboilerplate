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

        public List<string> GetAllPossibleValues(int dynamicParameterId)
        {
            return new List<string>();
        }

        public Task<List<string>> GetAllPossibleValuesAsync(int dynamicParameterId)
        {
            return Task.FromResult(new List<string>());
        }
    }
}
