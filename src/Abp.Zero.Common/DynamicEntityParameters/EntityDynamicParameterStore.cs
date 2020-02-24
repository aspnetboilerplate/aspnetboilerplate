using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterStore : IEntityDynamicParameterStore, ITransientDependency
    {
        private readonly IRepository<EntityDynamicParameter> _entityDynamicParameterRepository;

        public EntityDynamicParameterStore(IRepository<EntityDynamicParameter> entityDynamicParameterRepository)
        {
            _entityDynamicParameterRepository = entityDynamicParameterRepository;
        }

        public virtual EntityDynamicParameter Get(int id)
        {
            return _entityDynamicParameterRepository.Get(id);
        }

        public virtual Task<EntityDynamicParameter> GetAsync(int id)
        {
            return _entityDynamicParameterRepository.GetAsync(id);
        }

        public virtual void Add(EntityDynamicParameter entityDynamicParameter)
        {
            _entityDynamicParameterRepository.Insert(entityDynamicParameter);
        }

        public virtual Task AddAsync(EntityDynamicParameter entityDynamicParameter)
        {
            return _entityDynamicParameterRepository.InsertAsync(entityDynamicParameter);
        }

        public virtual void Update(EntityDynamicParameter entityDynamicParameter)
        {
            _entityDynamicParameterRepository.Update(entityDynamicParameter);
        }

        public virtual Task UpdateAsync(EntityDynamicParameter entityDynamicParameter)
        {
            return _entityDynamicParameterRepository.UpdateAsync(entityDynamicParameter);
        }

        public virtual void Delete(int id)
        {
            _entityDynamicParameterRepository.Delete(id);
        }

        public virtual Task DeleteAsync(int id)
        {
            return _entityDynamicParameterRepository.DeleteAsync(id);
        }
    }
}
