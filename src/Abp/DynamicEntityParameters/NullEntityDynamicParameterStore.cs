using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public class NullEntityDynamicParameterStore : IEntityDynamicParameterStore
    {
        public static NullEntityDynamicParameterStore Instance = new NullEntityDynamicParameterStore();

        public EntityDynamicParameter Get(int id)
        {
            return default;
        }

        public Task<EntityDynamicParameter> GetAsync(int id)
        {
            return Task.FromResult<EntityDynamicParameter>(default);
        }

        public List<EntityDynamicParameter> GetAllParameters(string entityFullName)
        {
            return new List<EntityDynamicParameter>();
        }

        public Task<List<EntityDynamicParameter>> GetAllParametersAsync(string entityFullName)
        {
            return Task.FromResult(new List<EntityDynamicParameter>());
        }

        public void Add(EntityDynamicParameter entityDynamicParameter)
        {
        }

        public Task AddAsync(EntityDynamicParameter entityDynamicParameter)
        {
            return Task.CompletedTask;
        }

        public void Update(EntityDynamicParameter entityDynamicParameter)
        {
        }

        public Task UpdateAsync(EntityDynamicParameter entityDynamicParameter)
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
