using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq;

namespace Abp.DynamicEntityProperties
{
    public class DynamicPropertyValueStore : IDynamicPropertyValueStore, ITransientDependency
    {
        private readonly IRepository<DynamicPropertyValue, long> _dynamicPropertyValuesRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public DynamicPropertyValueStore(
            IRepository<DynamicPropertyValue, long> dynamicPropertyValuesRepository,
            IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _dynamicPropertyValuesRepository = dynamicPropertyValuesRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public virtual DynamicPropertyValue Get(long id)
        {
            return _dynamicPropertyValuesRepository.Get(id);
        }

        public virtual Task<DynamicPropertyValue> GetAsync(long id)
        {
            return _dynamicPropertyValuesRepository.GetAsync(id);
        }

        public virtual List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(int dynamicPropertyId)
        {
            return _dynamicPropertyValuesRepository.GetAll()
                .Where(propertyValue => propertyValue.DynamicPropertyId == dynamicPropertyId).ToList();
        }

        public virtual Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(int dynamicPropertyId)
        {
            return _asyncQueryableExecuter.ToListAsync(_dynamicPropertyValuesRepository.GetAll()
                .Where(propertyValue => propertyValue.DynamicPropertyId == dynamicPropertyId));
        }

        public virtual void Add(DynamicPropertyValue dynamicPropertyValue)
        {
            _dynamicPropertyValuesRepository.Insert(dynamicPropertyValue);
        }

        public virtual Task AddAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            return _dynamicPropertyValuesRepository.InsertAsync(dynamicPropertyValue);
        }

        public virtual void Update(DynamicPropertyValue dynamicPropertyValue)
        {
            _dynamicPropertyValuesRepository.Update(dynamicPropertyValue);
        }

        public virtual Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            return _dynamicPropertyValuesRepository.UpdateAsync(dynamicPropertyValue);
        }

        public virtual void Delete(long id)
        {
            _dynamicPropertyValuesRepository.Delete(id);
        }

        public virtual Task DeleteAsync(long id)
        {
            return _dynamicPropertyValuesRepository.DeleteAsync(id);
        }

        public virtual void CleanValues(int dynamicPropertyId)
        {
            _dynamicPropertyValuesRepository.Delete(value => value.DynamicPropertyId == dynamicPropertyId);
        }

        public virtual Task CleanValuesAsync(int dynamicPropertyId)
        {
            return _dynamicPropertyValuesRepository.DeleteAsync(value => value.DynamicPropertyId == dynamicPropertyId);
        }
    }
}
