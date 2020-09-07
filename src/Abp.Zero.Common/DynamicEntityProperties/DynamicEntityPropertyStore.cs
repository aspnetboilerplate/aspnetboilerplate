using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Linq;

namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyStore : IDynamicEntityPropertyStore, ITransientDependency
    {
        private readonly IRepository<DynamicEntityProperty> _dynamicEntityPropertyRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;

        public DynamicEntityPropertyStore(IRepository<DynamicEntityProperty> dynamicEntityPropertyRepository, IAsyncQueryableExecuter asyncQueryableExecuter)
        {
            _dynamicEntityPropertyRepository = dynamicEntityPropertyRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
        }

        public virtual DynamicEntityProperty Get(int id)
        {
            return _dynamicEntityPropertyRepository.Get(id);
        }

        public virtual Task<DynamicEntityProperty> GetAsync(int id)
        {
            return _dynamicEntityPropertyRepository.GetAsync(id);
        }

        public List<DynamicEntityProperty> GetAll()
        {
            return _dynamicEntityPropertyRepository.GetAllIncluding(e => e.DynamicProperty)
                .OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicPropertyId).ToList();
        }

        public Task<List<DynamicEntityProperty>> GetAllAsync()
        {
            return _asyncQueryableExecuter.ToListAsync(
                _dynamicEntityPropertyRepository.GetAllIncluding(e => e.DynamicProperty)
                    .OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicPropertyId));
        }

        public virtual List<DynamicEntityProperty> GetAll(string entityFullName)
        {
            return _dynamicEntityPropertyRepository.GetAllIncluding(e => e.DynamicProperty)
                .Where(x => x.EntityFullName == entityFullName).OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicPropertyId).ToList();
        }

        public virtual Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName)
        {
            return _asyncQueryableExecuter.ToListAsync(
                _dynamicEntityPropertyRepository.GetAllIncluding(e => e.DynamicProperty)
                    .Where(x => x.EntityFullName == entityFullName)
                    .OrderBy(edp => edp.EntityFullName).ThenBy(edp => edp.DynamicPropertyId));
        }

        public virtual void Add(DynamicEntityProperty dynamicEntityProperty)
        {
            _dynamicEntityPropertyRepository.Insert(dynamicEntityProperty);
        }

        public virtual Task AddAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            return _dynamicEntityPropertyRepository.InsertAsync(dynamicEntityProperty);
        }

        public virtual void Update(DynamicEntityProperty dynamicEntityProperty)
        {
            _dynamicEntityPropertyRepository.Update(dynamicEntityProperty);
        }

        public virtual Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            return _dynamicEntityPropertyRepository.UpdateAsync(dynamicEntityProperty);
        }

        public virtual void Delete(int id)
        {
            _dynamicEntityPropertyRepository.Delete(id);
        }

        public virtual Task DeleteAsync(int id)
        {
            return _dynamicEntityPropertyRepository.DeleteAsync(id);
        }
    }
}
