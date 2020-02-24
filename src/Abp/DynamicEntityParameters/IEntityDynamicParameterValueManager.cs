using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IEntityDynamicParameterValueManager
    {
        EntityDynamicParameterValue Get(int id);

        Task<EntityDynamicParameterValue> GetAsync(int id);

        void Add(EntityDynamicParameterValue dynamicParameterValue);

        Task AddAsync(EntityDynamicParameterValue dynamicParameterValue);

        void Update(EntityDynamicParameterValue dynamicParameterValue);

        Task UpdateAsync(EntityDynamicParameterValue dynamicParameterValue);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
