using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq;

namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyStore : IDynamicEntityPropertyStore, ITransientDependency
    {
        private readonly IRepository<DynamicEntityProperty> _dynamicEntityPropertyRepository;
        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public DynamicEntityPropertyStore(
            IRepository<DynamicEntityProperty> dynamicEntityPropertyRepository,
            IAsyncQueryableExecuter asyncQueryableExecuter,
            IUnitOfWorkManager unitOfWorkManager)
        {
            _dynamicEntityPropertyRepository = dynamicEntityPropertyRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public virtual DynamicEntityProperty Get(int id)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
                _dynamicEntityPropertyRepository.Get(id)
            );
        }

        public virtual async Task<DynamicEntityProperty> GetAsync(int id)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicEntityPropertyRepository.GetAsync(id)
            );
        }

        public List<DynamicEntityProperty> GetAll()
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _dynamicEntityPropertyRepository
                    .GetAllIncluding(e => e.DynamicProperty)
                    .OrderBy(edp => edp.EntityFullName)
                    .ThenBy(edp => edp.DynamicPropertyId)
                    .ToList();
            });
        }

        public async Task<List<DynamicEntityProperty>> GetAllAsync()
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _asyncQueryableExecuter.ToListAsync(
                    _dynamicEntityPropertyRepository
                        .GetAllIncluding(e => e.DynamicProperty)
                        .OrderBy(edp => edp.EntityFullName)
                        .ThenBy(edp => edp.DynamicPropertyId)
                );
            });
        }

        public virtual List<DynamicEntityProperty> GetAll(string entityFullName)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                return _dynamicEntityPropertyRepository
                    .GetAllIncluding(e => e.DynamicProperty)
                    .Where(x => x.EntityFullName == entityFullName)
                    .OrderBy(edp => edp.EntityFullName)
                    .ThenBy(edp => edp.DynamicPropertyId)
                    .ToList();
            });
        }

        public virtual async Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                return await _asyncQueryableExecuter.ToListAsync(
                    _dynamicEntityPropertyRepository
                        .GetAllIncluding(e => e.DynamicProperty)
                        .Where(x => x.EntityFullName == entityFullName)
                        .OrderBy(edp => edp.EntityFullName)
                        .ThenBy(edp => edp.DynamicPropertyId)
                );
            });
        }

        public virtual void Add(DynamicEntityProperty dynamicEntityProperty)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _dynamicEntityPropertyRepository.Insert(dynamicEntityProperty);
            });
        }

        public virtual async Task AddAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicEntityPropertyRepository.InsertAsync(dynamicEntityProperty)
            );
        }

        public virtual void Update(DynamicEntityProperty dynamicEntityProperty)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _dynamicEntityPropertyRepository.Update(dynamicEntityProperty);
            });
        }

        public virtual async Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicEntityPropertyRepository.UpdateAsync(dynamicEntityProperty)
            );
        }

        public virtual void Delete(int id)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                _dynamicEntityPropertyRepository.Delete(id);
            });
        }

        public virtual async Task DeleteAsync(int id)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
                await _dynamicEntityPropertyRepository.DeleteAsync(id)
            );
        }
    }
}
