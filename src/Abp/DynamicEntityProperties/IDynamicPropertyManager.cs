using System.Threading.Tasks;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicPropertyManager
    {
        DynamicProperty Get(int id);

        Task<DynamicProperty> GetAsync(int id);

        DynamicProperty Get(string propertyName);

        Task<DynamicProperty> GetAsync(string propertyName);

        DynamicProperty Add(DynamicProperty dynamicProperty);

        Task<DynamicProperty> AddAsync(DynamicProperty dynamicProperty);

        DynamicProperty Update(DynamicProperty dynamicProperty);

        Task<DynamicProperty> UpdateAsync(DynamicProperty dynamicProperty);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
