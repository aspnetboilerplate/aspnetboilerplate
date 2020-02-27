using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterStore : IEntityDynamicParameterStore, ITransientDependency
    {
        private readonly IRepository<EntityDynamicParameter> _entityDynamicParameterRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public EntityDynamicParameterStore(IRepository<EntityDynamicParameter> entityDynamicParameterRepository, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _entityDynamicParameterRepository = entityDynamicParameterRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public virtual EntityDynamicParameter Get(int id)
        {
            return _entityDynamicParameterRepository.Get(id);
        }

        public virtual Task<EntityDynamicParameter> GetAsync(int id)
        {
            return _entityDynamicParameterRepository.GetAsync(id);
        }

        public virtual List<EntityDynamicParameter> GetAllParameters(string entityFullName)
        {
            return _entityDynamicParameterRepository.GetAll().Where(x => x.EntityFullName == entityFullName).ToList();
        }

        public virtual Task<List<EntityDynamicParameter>> GetAllParametersAsync(string entityFullName)
        {
            return _asyncQueryableExecuter.ToListAsync(_entityDynamicParameterRepository.GetAll().Where(x => x.EntityFullName == entityFullName));
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
