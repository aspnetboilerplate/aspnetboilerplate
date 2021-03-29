using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;

namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyManager : IDynamicEntityPropertyManager, ITransientDependency
    {
        private readonly IDynamicPropertyPermissionChecker _dynamicPropertyPermissionChecker;
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;

        public IDynamicEntityPropertyStore DynamicEntityPropertyStore { get; set; }

        public const string CacheName = "AbpZeroDynamicEntityPropertyCache";

        private ITypedCache<int, DynamicEntityProperty> DynamicEntityPropertyCache => _cacheManager.GetCache<int, DynamicEntityProperty>(CacheName);

        public DynamicEntityPropertyManager(
            IDynamicPropertyPermissionChecker dynamicPropertyPermissionChecker,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager
            )
        {
            _dynamicPropertyPermissionChecker = dynamicPropertyPermissionChecker;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;

            DynamicEntityPropertyStore = NullDynamicEntityPropertyStore.Instance;
        }

        private void CheckEntityName(string entityFullName)
        {
            if (!_dynamicEntityPropertyDefinitionManager.ContainsEntity(entityFullName))
            {
                throw new ApplicationException($"Unknown entity {entityFullName}.");
            }
        }

        public virtual DynamicEntityProperty Get(int id)
        {
            var entityProperty = DynamicEntityPropertyCache.Get(id, () => DynamicEntityPropertyStore.Get(id));
            _dynamicPropertyPermissionChecker.CheckPermission(entityProperty.DynamicPropertyId);
            return entityProperty;
        }

        public virtual async Task<DynamicEntityProperty> GetAsync(int id)
        {
            var entityProperty = await DynamicEntityPropertyCache.GetAsync(id, () => DynamicEntityPropertyStore.GetAsync(id));
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(entityProperty.DynamicPropertyId);
            return entityProperty;
        }

        public List<DynamicEntityProperty> GetAll(string entityFullName)
        {
            var allProperties = DynamicEntityPropertyStore.GetAll(entityFullName);
            allProperties = allProperties
                .Where(dynamicEntityProperty =>
                    _dynamicPropertyPermissionChecker.IsGranted(dynamicEntityProperty.DynamicPropertyId))
                .ToList();
            return allProperties;
        }

        public async Task<List<DynamicEntityProperty>> GetAllAsync(string entityFullName)
        {
            var allProperties = await DynamicEntityPropertyStore.GetAllAsync(entityFullName);

            var controlledProperties = new List<DynamicEntityProperty>();
            foreach (var dynamicEntityProperty in allProperties)
            {
                if (await _dynamicPropertyPermissionChecker.IsGrantedAsync(dynamicEntityProperty.DynamicPropertyId))
                {
                    controlledProperties.Add(dynamicEntityProperty);
                }
            }
            return controlledProperties;
        }

        public List<DynamicEntityProperty> GetAll()
        {
            var allProperties = DynamicEntityPropertyStore.GetAll();
            allProperties = allProperties
                .Where(dynamicEntityProperty =>
                    _dynamicPropertyPermissionChecker.IsGranted(dynamicEntityProperty.DynamicPropertyId))
                .ToList();
            return allProperties;
        }

        public async Task<List<DynamicEntityProperty>> GetAllAsync()
        {
            var allProperties = await DynamicEntityPropertyStore.GetAllAsync();

            var controlledProperties = new List<DynamicEntityProperty>();
            foreach (var dynamicEntityProperty in allProperties)
            {
                if (await _dynamicPropertyPermissionChecker.IsGrantedAsync(dynamicEntityProperty.DynamicPropertyId))
                {
                    controlledProperties.Add(dynamicEntityProperty);
                }
            }
            return controlledProperties;
        }

        public virtual void Add(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicEntityProperty.DynamicPropertyId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                DynamicEntityPropertyStore.Add(dynamicEntityProperty);
                uow.Complete();
            }

            DynamicEntityPropertyCache.Set(dynamicEntityProperty.Id, dynamicEntityProperty);
        }

        public virtual async Task AddAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicEntityProperty.DynamicPropertyId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await DynamicEntityPropertyStore.AddAsync(dynamicEntityProperty);
                await uow.CompleteAsync();
            }
            await DynamicEntityPropertyCache.SetAsync(dynamicEntityProperty.Id, dynamicEntityProperty);
        }

        public virtual void Update(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicEntityProperty.DynamicPropertyId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                DynamicEntityPropertyStore.Update(dynamicEntityProperty);
                uow.Complete();
            }
            DynamicEntityPropertyCache.Set(dynamicEntityProperty.Id, dynamicEntityProperty);
        }

        public virtual async Task UpdateAsync(DynamicEntityProperty dynamicEntityProperty)
        {
            CheckEntityName(dynamicEntityProperty.EntityFullName);
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicEntityProperty.DynamicPropertyId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await DynamicEntityPropertyStore.UpdateAsync(dynamicEntityProperty);
                await uow.CompleteAsync();
            }
            await DynamicEntityPropertyCache.SetAsync(dynamicEntityProperty.Id, dynamicEntityProperty);
        }

        public virtual void Delete(int id)
        {
            var dynamicEntityProperty = Get(id);//Get checks permission, no need to check it again
            if (dynamicEntityProperty == null)
            {
                return;
            }
            DynamicEntityPropertyStore.Delete(dynamicEntityProperty.Id);
            DynamicEntityPropertyCache.Remove(id);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var dynamicEntityProperty = await GetAsync(id);//Get checks permission, no need to check it again
            if (dynamicEntityProperty == null)
            {
                return;
            }
            await DynamicEntityPropertyStore.DeleteAsync(dynamicEntityProperty.Id);
            await DynamicEntityPropertyCache.RemoveAsync(id);
        }
    }
}
