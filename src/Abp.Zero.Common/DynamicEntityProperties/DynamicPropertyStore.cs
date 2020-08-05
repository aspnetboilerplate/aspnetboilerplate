using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;

namespace Abp.DynamicEntityProperties
{
    public class DynamicPropertyStore : IDynamicPropertyStore, ITransientDependency
    {
        private readonly IRepository<DynamicProperty> _dynamicPropertyRepository;

        public DynamicPropertyStore(IRepository<DynamicProperty> dynamicPropertyRepository)
        {
            _dynamicPropertyRepository = dynamicPropertyRepository;
        }

        public virtual DynamicProperty Get(int id)
        {
            return _dynamicPropertyRepository.Get(id);
        }

        public virtual Task<DynamicProperty> GetAsync(int id)
        {
            return _dynamicPropertyRepository.GetAsync(id);
        }

        public virtual DynamicProperty Get(string propertyName)
        {
            return _dynamicPropertyRepository.FirstOrDefault(x => x.PropertyName == propertyName);
        }

        public virtual Task<DynamicProperty> GetAsync(string propertyName)
        {
            return _dynamicPropertyRepository.FirstOrDefaultAsync(x => x.PropertyName == propertyName);
        }

        public virtual List<DynamicProperty> GetAll()
        {
            return _dynamicPropertyRepository.GetAllList();
        }

        public virtual async Task<List<DynamicProperty>> GetAllAsync()
        {
            return await _dynamicPropertyRepository.GetAllListAsync();
        }

        public virtual void Add(DynamicProperty dynamicProperty)
        {
            _dynamicPropertyRepository.Insert(dynamicProperty);
        }

        public virtual Task AddAsync(DynamicProperty dynamicProperty)
        {
            return _dynamicPropertyRepository.InsertAsync(dynamicProperty);
        }

        public virtual void Update(DynamicProperty dynamicProperty)
        {
            _dynamicPropertyRepository.Update(dynamicProperty);
        }

        public virtual Task UpdateAsync(DynamicProperty dynamicProperty)
        {
            return _dynamicPropertyRepository.UpdateAsync(dynamicProperty);
        }

        public virtual void Delete(int id)
        {
            _dynamicPropertyRepository.Delete(id);
        }

        public virtual Task DeleteAsync(int id)
        {
            return _dynamicPropertyRepository.DeleteAsync(id);
        }
    }
}
