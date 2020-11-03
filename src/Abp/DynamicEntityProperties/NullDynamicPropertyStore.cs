using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public class NullDynamicPropertyStore : IDynamicPropertyStore
    {
        public static NullDynamicPropertyStore Instance = new NullDynamicPropertyStore();

        public DynamicProperty Get(int id)
        {
            return default;
        }

        public Task<DynamicProperty> GetAsync(int id)
        {
            return Task.FromResult<DynamicProperty>(default);
        }

        public DynamicProperty Get(string propertyName)
        {
            return default;
        }

        public Task<DynamicProperty> GetAsync(string propertyName)
        {
            return Task.FromResult<DynamicProperty>(default);
        }

        public List<DynamicProperty> GetAll()
        {
            return new List<DynamicProperty>();
        }

        public Task<List<DynamicProperty>> GetAllAsync()
        {
            return Task.FromResult(new List<DynamicProperty>());
        }

        public void Add(DynamicProperty dynamicProperty)
        {
        }

        public Task AddAsync(DynamicProperty dynamicProperty)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicProperty dynamicProperty)
        {
        }

        public Task UpdateAsync(DynamicProperty dynamicProperty)
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
