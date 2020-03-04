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

        public List<EntityDynamicParameter> GetAll()
        {
            return _entityDynamicParameterRepository.GetAllIncluding(e => e.DynamicParameter)
                .OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicParameterId).ToList();
        }

        public Task<List<EntityDynamicParameter>> GetAllAsync()
        {
            return _asyncQueryableExecuter.ToListAsync(
                _entityDynamicParameterRepository.GetAllIncluding(e => e.DynamicParameter)
                    .OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicParameterId));
        }

        public virtual List<EntityDynamicParameter> GetAll(string entityFullName)
        {
            return _entityDynamicParameterRepository.GetAllIncluding(e => e.DynamicParameter)
                .Where(x => x.EntityFullName == entityFullName).OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicParameterId).ToList();
        }

        public virtual Task<List<EntityDynamicParameter>> GetAllAsync(string entityFullName)
        {
            return _asyncQueryableExecuter.ToListAsync(
                _entityDynamicParameterRepository.GetAllIncluding(e => e.DynamicParameter)
                    .Where(x => x.EntityFullName == entityFullName)
                    .OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicParameterId));
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
