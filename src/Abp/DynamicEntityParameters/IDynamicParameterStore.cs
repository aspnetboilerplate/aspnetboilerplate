using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicParameterStore
    {
        DynamicParameter Get(int id);

        Task<DynamicParameter> GetAsync(int id);

        void Add(DynamicParameter dynamicParameter);

        Task AddAsync(DynamicParameter dynamicParameter);

        void Update(DynamicParameter dynamicParameter);

        Task UpdateAsync(DynamicParameter dynamicParameter);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
