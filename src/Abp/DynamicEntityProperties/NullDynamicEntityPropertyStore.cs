using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public class NullDynamicEntityPropertyStore : IDynamicEntityPropertyStore
    {
        public static NullDynamicEntityPropertyStore Instance = new NullDynamicEntityPropertyStore();

        public DynamicEntityProperty Get(int id)
        {
            return default;
        }

        public Task<DynamicEntityProperty> GetAsync(int id)
        {
            return Task.FromResult<DynamicEntityProperty>(default);
        }

        public List<DynamicEntityProperty> GetAll(string entityFullName)
        {
            return new List<DynamicEntityProperty>();
        }

        public Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName)
        {
            return Task.FromResult(new List<DynamicEntityProperty>());
        }

        public List<DynamicEntityProperty> GetAll()
        {
            return new List<DynamicEntityProperty>();
        }

        public Task<List<DynamicEntityProperty>> GetAllAsync()
        {
            return Task.FromResult(new List<DynamicEntityProperty>());
        }

        public void Add(DynamicEntityProperty dynamicEntityProperty)
        {
        }

        public Task AddAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            return Task.CompletedTask;
        }

        public void Update(DynamicEntityProperty dynamicEntityProperty)
        {
        }

        public Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty)
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
