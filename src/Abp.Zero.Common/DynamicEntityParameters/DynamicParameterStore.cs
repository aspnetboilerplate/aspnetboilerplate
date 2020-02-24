using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Abp.DynamicEntityParameters
{
    public class DynamicParameterStore : IDynamicParameterStore, ITransientDependency
    {
        private readonly IRepository<DynamicParameter> _dynamicParameterRepository;

        public DynamicParameterStore(IRepository<DynamicParameter> dynamicParameterRepository)
        {
            _dynamicParameterRepository = dynamicParameterRepository;
        }

        public virtual DynamicParameter Get(int id)
        {
            return _dynamicParameterRepository.Get(id);
        }

        public virtual Task<DynamicParameter> GetAsync(int id)
        {
            return _dynamicParameterRepository.GetAsync(id);
        }

        public virtual void Add(DynamicParameter dynamicParameter)
        {
            _dynamicParameterRepository.Insert(dynamicParameter);
        }

        public virtual Task AddAsync(DynamicParameter dynamicParameter)
        {
            return _dynamicParameterRepository.InsertAsync(dynamicParameter);
        }

        public virtual void Update(DynamicParameter dynamicParameter)
        {
            _dynamicParameterRepository.Update(dynamicParameter);
        }

        public virtual Task UpdateAsync(DynamicParameter dynamicParameter)
        {
            return _dynamicParameterRepository.UpdateAsync(dynamicParameter);
        }

        public virtual void Delete(int id)
        {
            _dynamicParameterRepository.Delete(id);
        }

        public virtual Task DeleteAsync(int id)
        {
            return _dynamicParameterRepository.DeleteAsync(id);
        }
    }
}
