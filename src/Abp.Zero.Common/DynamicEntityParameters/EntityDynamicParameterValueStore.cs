using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterValueStore : IEntityDynamicParameterValueStore, ITransientDependency
    {
        private readonly IRepository<EntityDynamicParameterValue> _entityDynamicParameterValueRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public EntityDynamicParameterValueStore(IRepository<EntityDynamicParameterValue> entityDynamicParameterValueRepository, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _entityDynamicParameterValueRepository = entityDynamicParameterValueRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public virtual EntityDynamicParameterValue Get(int id)
        {
            return _entityDynamicParameterValueRepository.Get(id);
        }

        public virtual Task<EntityDynamicParameterValue> GetAsync(int id)
        {
            return _entityDynamicParameterValueRepository.GetAsync(id);
        }

        public virtual void Add(EntityDynamicParameterValue dynamicParameterValue)
        {
            _entityDynamicParameterValueRepository.Insert(dynamicParameterValue);
        }

        public virtual Task AddAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            return _entityDynamicParameterValueRepository.InsertAsync(dynamicParameterValue);
        }

        public virtual void Update(EntityDynamicParameterValue dynamicParameterValue)
        {
            _entityDynamicParameterValueRepository.Update(dynamicParameterValue);
        }

        public virtual Task UpdateAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            return _entityDynamicParameterValueRepository.UpdateAsync(dynamicParameterValue);
        }

        public virtual void Delete(int id)
        {
            _entityDynamicParameterValueRepository.Delete(id);
        }

        public virtual Task DeleteAsync(int id)
        {
            return _entityDynamicParameterValueRepository.DeleteAsync(id);
        }

        public virtual List<EntityDynamicParameterValue> GetValues(string entityRowId, int parameterId)
        {
            return _entityDynamicParameterValueRepository.GetAll().Where(val =>
                val.EntityRowId == entityRowId && val.EntityDynamicParameterId == parameterId).ToList();
        }

        public virtual Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityRowId, int parameterId)
        {
            return _asyncQueryableExecuter.ToListAsync(_entityDynamicParameterValueRepository.GetAll().Where(val =>
                val.EntityRowId == entityRowId && val.EntityDynamicParameterId == parameterId));
        }

        public virtual void CleanValues(string entityRowId, int parameterId)
        {
            var list = _entityDynamicParameterValueRepository.GetAll().Where(val =>
                 val.EntityRowId == entityRowId && val.EntityDynamicParameterId == parameterId).ToList();

            foreach (var entityDynamicParameterValue in list)
            {
                _entityDynamicParameterValueRepository.Delete(entityDynamicParameterValue);
            }
        }

        public virtual async Task CleanValuesAsync(string entityRowId, int parameterId)
        {
            var list = await _asyncQueryableExecuter.ToListAsync(_entityDynamicParameterValueRepository.GetAll().Where(val =>
                 val.EntityRowId == entityRowId && val.EntityDynamicParameterId == parameterId));

            foreach (var entityDynamicParameterValue in list)
            {
                await _entityDynamicParameterValueRepository.DeleteAsync(entityDynamicParameterValue);
            }
        }
    }
}
