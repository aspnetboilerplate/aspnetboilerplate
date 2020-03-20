using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq;

namespace Abp.DynamicEntityParameters
{
    public class DynamicParameterValueStore : IDynamicParameterValueStore, ITransientDependency
    {
        private readonly IRepository<DynamicParameterValue> _dynamicParameterValuesRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public DynamicParameterValueStore(IRepository<DynamicParameterValue> dynamicParameterValuesRepository, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _dynamicParameterValuesRepository = dynamicParameterValuesRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
        }
        public virtual DynamicParameterValue Get(int id)
        {
            return _dynamicParameterValuesRepository.Get(id);
        }

        public virtual Task<DynamicParameterValue> GetAsync(int id)
        {
            return _dynamicParameterValuesRepository.GetAsync(id);
        }

        public virtual List<DynamicParameterValue> GetAllValuesOfDynamicParameter(int dynamicParameterId)
        {
            return _dynamicParameterValuesRepository.GetAll()
                .Where(parameterValue => parameterValue.DynamicParameterId == dynamicParameterId).ToList();
        }

        public virtual Task<List<DynamicParameterValue>> GetAllValuesOfDynamicParameterAsync(int dynamicParameterId)
        {
            return _asyncQueryableExecuter.ToListAsync(_dynamicParameterValuesRepository.GetAll()
                .Where(parameterValue => parameterValue.DynamicParameterId == dynamicParameterId));
        }

        public virtual void Add(DynamicParameterValue dynamicParameterValue)
        {
            _dynamicParameterValuesRepository.Insert(dynamicParameterValue);
        }

        public virtual Task AddAsync(DynamicParameterValue dynamicParameterValue)
        {
            return _dynamicParameterValuesRepository.InsertAsync(dynamicParameterValue);
        }

        public virtual void Update(DynamicParameterValue dynamicParameterValue)
        {
            _dynamicParameterValuesRepository.Update(dynamicParameterValue);
        }

        public virtual Task UpdateAsync(DynamicParameterValue dynamicParameterValue)
        {
            return _dynamicParameterValuesRepository.UpdateAsync(dynamicParameterValue);
        }

        public virtual void Delete(int id)
        {
            _dynamicParameterValuesRepository.Delete(id);
        }

        public virtual Task DeleteAsync(int id)
        {
            return _dynamicParameterValuesRepository.DeleteAsync(id);
        }

        public virtual void CleanValues(int dynamicParameterId)
        {
            _dynamicParameterValuesRepository.Delete(value => value.DynamicParameterId == dynamicParameterId);
        }

        public virtual Task CleanValuesAsync(int dynamicParameterId)
        {
            return _dynamicParameterValuesRepository.DeleteAsync(value => value.DynamicParameterId == dynamicParameterId);
        }
    }
}
