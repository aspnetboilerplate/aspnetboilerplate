using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyManager : IDynamicEntityPropertyManager, ITransientDependency
    {
        private readonly IDynamicPropertyPermissionChecker _dynamicPropertyPermissionChecker;
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;

        public IDynamicEntityPropertyStore DynamicEntityPropertyStore { get; set; }
        public IAbpSession AbpSession { get; set; }

        public const string CacheName = "AbpZeroDynamicEntityPropertyCache";

        private ITypedCache<string, DynamicEntityProperty> DynamicEntityPropertyCache =>
            _cacheManager.GetCache<string, DynamicEntityProperty>(CacheName);

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
            AbpSession = NullAbpSession.Instance;
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
            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);

            var entityProperty = DynamicEntityPropertyCache.Get(cacheKey, () => DynamicEntityPropertyStore.Get(id));
            _dynamicPropertyPermissionChecker.CheckPermission(entityProperty.DynamicPropertyId);
            return entityProperty;
        }

        public virtual async Task<DynamicEntityProperty> GetAsync(int id)
        {
            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);

            var entityProperty =
                await DynamicEntityPropertyCache.GetAsync(cacheKey, () => DynamicEntityPropertyStore.GetAsync(id));
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(entityProperty.DynamicPropertyId);
            return entityProperty;
        }

        public List<DynamicEntityProperty> GetAll(string entityFullName)
        {
            var allProperties = DynamicEntityPropertyStore.GetAll(entityFullName);
            allProperties = allProperties.Where(dynamicEntityProperty =>
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
            allProperties = allProperties.Where(dynamicEntityProperty =>
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

            var cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.TenantId);
            DynamicEntityPropertyCache.Set(cacheKey, dynamicEntityProperty);
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

            var cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.TenantId);
            await DynamicEntityPropertyCache.SetAsync(cacheKey, dynamicEntityProperty);
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

            var cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.TenantId);
            DynamicEntityPropertyCache.Set(cacheKey, dynamicEntityProperty);
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

            var cacheKey = GetCacheKey(dynamicEntityProperty.Id, dynamicEntityProperty.TenantId);
            await DynamicEntityPropertyCache.SetAsync(cacheKey, dynamicEntityProperty);
        }

        public virtual void Delete(int id)
        {
            var dynamicEntityProperty = Get(id); //Get checks permission, no need to check it again
            if (dynamicEntityProperty == null)
            {
                return;
            }

            DynamicEntityPropertyStore.Delete(dynamicEntityProperty.Id);
            
            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);
            
            DynamicEntityPropertyCache.Remove(cacheKey);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var dynamicEntityProperty = await GetAsync(id); //Get checks permission, no need to check it again
            if (dynamicEntityProperty == null)
            {
                return;
            }

            await DynamicEntityPropertyStore.DeleteAsync(dynamicEntityProperty.Id);
            
            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);
            
            await DynamicEntityPropertyCache.RemoveAsync(cacheKey);
        }

        protected virtual int? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }

        protected virtual string GetCacheKey(int id, int? tenantId)
        {
            return id + "@" + (tenantId ?? 0);
        }
    }
}
