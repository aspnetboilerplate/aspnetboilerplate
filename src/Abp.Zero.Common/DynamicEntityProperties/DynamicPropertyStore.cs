using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.DynamicEntityProperties
{
    public class DynamicPropertyStore : IDynamicPropertyStore, ITransientDependency
    {
        private readonly IRepository<DynamicProperty> _dynamicPropertyRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DynamicPropertyStore(
            IRepository<DynamicProperty> dynamicPropertyRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _dynamicPropertyRepository = dynamicPropertyRepository;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual DynamicProperty Get(int id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
                _dynamicPropertyRepository.Get(id)
            );
        }

        public virtual async Task<DynamicProperty> GetAsync(int id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicPropertyRepository.GetAsync(id)
            );
        }

        public virtual DynamicProperty Get(string propertyName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _dynamicPropertyRepository.FirstOrDefault(x => x.PropertyName == propertyName);
            });
        }

        public virtual async Task<DynamicProperty> GetAsync(string propertyName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _dynamicPropertyRepository.FirstOrDefaultAsync(x => x.PropertyName == propertyName);
            });
        }

        public virtual List<DynamicProperty> GetAll()
        {
            return _unitOfWorkManager.WithUnitOfWork(() => _dynamicPropertyRepository.GetAllList());
        }

        public virtual async Task<List<DynamicProperty>> GetAllAsync()
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicPropertyRepository.GetAllListAsync()
            );
        }

        public virtual void Add(DynamicProperty dynamicProperty)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _dynamicPropertyRepository.Insert(dynamicProperty);
            });
        }

        public virtual async Task AddAsync(DynamicProperty dynamicProperty)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicPropertyRepository.InsertAsync(dynamicProperty)
            );
        }

        public virtual void Update(DynamicProperty dynamicProperty)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _dynamicPropertyRepository.Update(dynamicProperty);
            });
        }

        public virtual async Task UpdateAsync(DynamicProperty dynamicProperty)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicPropertyRepository.UpdateAsync(dynamicProperty)
            );
        }

        public virtual void Delete(int id)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _dynamicPropertyRepository.Delete(id);
            });
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicPropertyRepository.DeleteAsync(id)
            );
        }
    }
}
