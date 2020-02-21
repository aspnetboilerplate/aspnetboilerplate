using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicParameterValueStore
    {
        DynamicParameterValues Get(int id);

        Task<DynamicParameterValues> GetAsync(int id);

        void Add(DynamicParameterValues dynamicParameterValue);

        Task AddAsync(DynamicParameterValues dynamicParameterValue);

        void Update(DynamicParameterValues dynamicParameterValue);

        Task UpdateAsync(DynamicParameterValues dynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
