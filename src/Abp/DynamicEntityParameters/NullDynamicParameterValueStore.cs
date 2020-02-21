using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public class NullDynamicParameterValueStore : IDynamicParameterValueStore
    {
        public static NullDynamicParameterValueStore Instance = new NullDynamicParameterValueStore();

        public DynamicParameterValues Get(int id)
        {
            return default;
        }

        public Task<DynamicParameterValues> GetAsync(int id)
        {
            return Task.FromResult<DynamicParameterValues>(default);
        }

        public void Add(DynamicParameterValues dynamicParameterValue)
        {
        }

        public Task AddAsync(DynamicParameterValues dynamicParameterValue)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicParameterValues dynamicParameterValue)
        {
        }

        public Task UpdateAsync(DynamicParameterValues dynamicParameterValue)
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
    }
}
