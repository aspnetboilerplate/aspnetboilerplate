using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Caching;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterManager : IEntityDynamicParameterManager, ITransientDependency
    {
        private readonly IDynamicParameterPermissionChecker _dynamicParameterPermissionChecker;
        private readonly ICacheManager _cacheManager;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public IEntityDynamicParameterStore EntityDynamicParameterStore { get; set; }

        public const string CacheName = "AbpZeroEntityDynamicParameterCache";

        private ITypedCache<int, EntityDynamicParameter> EntityDynamicParameterCache => _cacheManager.GetCache<int, EntityDynamicParameter>(CacheName);

        public EntityDynamicParameterManager(
            IDynamicParameterPermissionChecker dynamicParameterPermissionChecker,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager
            )
        {
            _dynamicParameterPermissionChecker = dynamicParameterPermissionChecker;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            EntityDynamicParameterStore = NullEntityDynamicParameterStore.Instance;
        }

        public virtual EntityDynamicParameter Get(int id)
        {
            var entityParameter = EntityDynamicParameterCache.Get(id, () =>
                {
                    return EntityDynamicParameterStore.Get(id);
                });
            _dynamicParameterPermissionChecker.CheckPermissions(entityParameter.DynamicParameterId);
            return entityParameter;
        }

        public virtual async Task<EntityDynamicParameter> GetAsync(int id)
        {
            var entityParameter = await EntityDynamicParameterCache.GetAsync(id, () => EntityDynamicParameterStore.GetAsync(id));
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(entityParameter.DynamicParameterId);
            return entityParameter;
        }

        public virtual void Add(EntityDynamicParameter entityDynamicParameter)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(entityDynamicParameter.DynamicParameterId);
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                EntityDynamicParameterStore.Add(entityDynamicParameter);
                uow.Complete();
            }

            EntityDynamicParameterCache.Set(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual async Task AddAsync(EntityDynamicParameter entityDynamicParameter)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(entityDynamicParameter.DynamicParameterId);
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await EntityDynamicParameterStore.AddAsync(entityDynamicParameter);
                uow.Complete();
            }
            await EntityDynamicParameterCache.SetAsync(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual void Update(EntityDynamicParameter entityDynamicParameter)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(entityDynamicParameter.DynamicParameterId);
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                EntityDynamicParameterStore.Update(entityDynamicParameter);
                uow.Complete();
            }
            EntityDynamicParameterCache.Set(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual async Task UpdateAsync(EntityDynamicParameter entityDynamicParameter)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(entityDynamicParameter.DynamicParameterId);
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await EntityDynamicParameterStore.UpdateAsync(entityDynamicParameter);
                uow.Complete();
            }
            await EntityDynamicParameterCache.SetAsync(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual void Delete(int id)
        {
            var entityDynamicParameter = Get(id);//Get checks permission, no need to check it again
            if (entityDynamicParameter == null)
            {
                return;
            }
            EntityDynamicParameterStore.Delete(entityDynamicParameter.Id);
            EntityDynamicParameterCache.Remove(id);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entityDynamicParameter = await GetAsync(id);//Get checks permission, no need to check it again
            if (entityDynamicParameter == null)
            {
                return;
            }
            await EntityDynamicParameterStore.DeleteAsync(entityDynamicParameter.Id);
            await EntityDynamicParameterCache.RemoveAsync(id);
        }
    }
}
