using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicPropertyManager
    {
        DynamicProperty Get(int id);

        Task<DynamicProperty> GetAsync(int id);

        DynamicProperty Get(string propertyName);

        Task<DynamicProperty> GetAsync(string propertyName);

        void Add(DynamicProperty dynamicProperty);

        Task AddAsync(DynamicProperty dynamicProperty);

        void Update(DynamicProperty dynamicProperty);

        Task UpdateAsync(DynamicProperty dynamicProperty);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
