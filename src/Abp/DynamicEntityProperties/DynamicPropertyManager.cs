using System;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;

namespace Abp.DynamicEntityProperties
{
    public class DynamicPropertyManager : IDynamicPropertyManager, ITransientDependency
    {
        private readonly ICacheManager _cacheManager;
        private readonly IDynamicPropertyStore _dynamicPropertyStore;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IDynamicEntityPropertyDefinitionManager _dynamicEntityPropertyDefinitionManager;

        public IAbpSession AbpSession { get; set; }
        
        public const string CacheName = "AbpZeroDynamicPropertyCache";
        private ITypedCache<string, DynamicProperty> DynamicPropertyCache => _cacheManager.GetCache<string, DynamicProperty>(CacheName);

        public DynamicPropertyManager(
            ICacheManager cacheManager,
            IDynamicPropertyStore dynamicPropertyStore,
            IUnitOfWorkManager unitOfWorkManager,
            IDynamicEntityPropertyDefinitionManager dynamicEntityPropertyDefinitionManager
        )
        {
            _cacheManager = cacheManager;
            _dynamicPropertyStore = dynamicPropertyStore;
            _unitOfWorkManager = unitOfWorkManager;
            _dynamicEntityPropertyDefinitionManager = dynamicEntityPropertyDefinitionManager;
            
            AbpSession = NullAbpSession.Instance;
        }

        public virtual DynamicProperty Get(int id)
        {
            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);
            
            return DynamicPropertyCache.Get(cacheKey, () => _dynamicPropertyStore.Get(id));
        }

        public virtual Task<DynamicProperty> GetAsync(int id)
        {
            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);
            
            return DynamicPropertyCache.GetAsync(cacheKey, (i) => _dynamicPropertyStore.GetAsync(id));
        }

        public virtual DynamicProperty Get(string propertyName)
        {
            return _dynamicPropertyStore.Get(propertyName);
        }

        public virtual Task<DynamicProperty> GetAsync(string propertyName)
        {
            return _dynamicPropertyStore.GetAsync(propertyName);
        }

        protected virtual void CheckDynamicProperty(DynamicProperty dynamicProperty, bool updating = false)
        {
            if (dynamicProperty == null)
            {
                throw new ArgumentNullException(nameof(dynamicProperty));
            }

            if (dynamicProperty.PropertyName.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(dynamicProperty.PropertyName));
            }

            if (!_dynamicEntityPropertyDefinitionManager.ContainsInputType(dynamicProperty.InputType))
            {
                throw new ApplicationException($"Input type is invalid, if you want to use \"{dynamicProperty.InputType}\" input type, define it in DynamicEntityPropertyDefinitionProvider.");
            }

            if(!updating)
            {
                var existingProperty = _dynamicPropertyStore.Get(dynamicProperty.PropertyName);
                if (existingProperty != null)
                {
                    throw new ArgumentException($"There is already a dynamic property with name: '{dynamicProperty.PropertyName}'");
                }
            }
        }

        public virtual DynamicProperty Add(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicPropertyStore.Add(dynamicProperty);
                uow.Complete();
            }

            var cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.TenantId);
            DynamicPropertyCache.Set(cacheKey, dynamicProperty);

            return dynamicProperty;
        }

        public virtual async Task<DynamicProperty> AddAsync(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicPropertyStore.AddAsync(dynamicProperty);
                await uow.CompleteAsync();
            }

            var cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.TenantId);
            await DynamicPropertyCache.SetAsync(cacheKey, dynamicProperty);
            
            return dynamicProperty;
        }

        public virtual DynamicProperty Update(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty, true);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicPropertyStore.Update(dynamicProperty);
                uow.Complete();
            }

            var cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.TenantId);
            DynamicPropertyCache.Set(cacheKey, dynamicProperty);
            
            return dynamicProperty;
        }

        public virtual async Task<DynamicProperty> UpdateAsync(DynamicProperty dynamicProperty)
        {
            CheckDynamicProperty(dynamicProperty, true);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicPropertyStore.UpdateAsync(dynamicProperty);
                await uow.CompleteAsync();
            }

            var cacheKey = GetCacheKey(dynamicProperty.Id, dynamicProperty.TenantId);
            await DynamicPropertyCache.SetAsync(cacheKey, dynamicProperty);
            
            return dynamicProperty;
        }

        public virtual void Delete(int id)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                _dynamicPropertyStore.Delete(id);
                uow.Complete();
            }

            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);
            
            DynamicPropertyCache.Remove(cacheKey);
        }

        public virtual async Task DeleteAsync(int id)
        {
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await _dynamicPropertyStore.DeleteAsync(id);
                await uow.CompleteAsync();
            }

            var tenantId = GetCurrentTenantId();
            var cacheKey = GetCacheKey(id, tenantId);
            
            await DynamicPropertyCache.RemoveAsync(cacheKey);
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
