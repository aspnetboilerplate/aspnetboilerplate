using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IEntityDynamicParameterManager
    {
        EntityDynamicParameter Get(int id);

        Task<EntityDynamicParameter> GetAsync(int id);

        void Add(EntityDynamicParameter entityDynamicParameter);

        Task AddAsync(EntityDynamicParameter entityDynamicParameter);

        void Update(EntityDynamicParameter entityDynamicParameter);

        Task UpdateAsync(EntityDynamicParameter entityDynamicParameter);

        void Delete(int id);

        Task DeleteAsync(int id);
    }
}
