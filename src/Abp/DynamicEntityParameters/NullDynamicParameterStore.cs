using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public class NullDynamicParameterStore : IDynamicParameterStore
    {
        public static NullDynamicParameterStore Instance = new NullDynamicParameterStore();

        public DynamicParameter Get(int id)
        {
            return default;
        }

        public Task<DynamicParameter> GetAsync(int id)
        {
            return Task.FromResult<DynamicParameter>(default);
        }

        public void Add(DynamicParameter dynamicParameter)
        {
        }

        public Task AddAsync(DynamicParameter dynamicParameter)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicParameter dynamicParameter)
        {
        }

        public Task UpdateAsync(DynamicParameter dynamicParameter)
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
