using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IDynamicEntityParameterDefinitionManager _dynamicEntityParameterDefinitionManager;

        public IEntityDynamicParameterStore EntityDynamicParameterStore { get; set; }

        public const string CacheName = "AbpZeroEntityDynamicParameterCache";

        private ITypedCache<int, EntityDynamicParameter> EntityDynamicParameterCache => _cacheManager.GetCache<int, EntityDynamicParameter>(CacheName);

        public EntityDynamicParameterManager(
            IDynamicParameterPermissionChecker dynamicParameterPermissionChecker,
            ICacheManager cacheManager,
            IUnitOfWorkManager unitOfWorkManager,
            IDynamicEntityParameterDefinitionManager dynamicEntityParameterDefinitionManager
            )
        {
            _dynamicParameterPermissionChecker = dynamicParameterPermissionChecker;
            _cacheManager = cacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _dynamicEntityParameterDefinitionManager = dynamicEntityParameterDefinitionManager;

            EntityDynamicParameterStore = NullEntityDynamicParameterStore.Instance;
        }

        private void CheckEntityName(string entityFullName)
        {
            if (!_dynamicEntityParameterDefinitionManager.ContainsEntity(entityFullName))
            {
                throw new ApplicationException($"Unknown entity {entityFullName}.");
            }
        }

        public virtual EntityDynamicParameter Get(int id)
        {
            var entityParameter = EntityDynamicParameterCache.Get(id, () => EntityDynamicParameterStore.Get(id));
            _dynamicParameterPermissionChecker.CheckPermission(entityParameter.DynamicParameterId);
            return entityParameter;
        }

        public virtual async Task<EntityDynamicParameter> GetAsync(int id)
        {
            var entityParameter = await EntityDynamicParameterCache.GetAsync(id, () => EntityDynamicParameterStore.GetAsync(id));
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(entityParameter.DynamicParameterId);
            return entityParameter;
        }

        public List<EntityDynamicParameter> GetAll(string entityFullName)
        {
            var allParameters = EntityDynamicParameterStore.GetAll(entityFullName);
            allParameters = allParameters
                .Where(entityDynamicParameter =>
                    _dynamicParameterPermissionChecker.IsGranted(entityDynamicParameter.DynamicParameterId))
                .ToList();
            return allParameters;
        }

        public async Task<List<EntityDynamicParameter>> GetAllAsync(string entityFullName)
        {
            var allParameters = await EntityDynamicParameterStore.GetAllAsync(entityFullName);

            var controlledParameters = new List<EntityDynamicParameter>();
            foreach (var entityDynamicParameter in allParameters)
            {
                if (await _dynamicParameterPermissionChecker.IsGrantedAsync(entityDynamicParameter.DynamicParameterId))
                {
                    controlledParameters.Add(entityDynamicParameter);
                }
            }
            return controlledParameters;
        }

        public List<EntityDynamicParameter> GetAll()
        {
            var allParameters = EntityDynamicParameterStore.GetAll();
            allParameters = allParameters
                .Where(entityDynamicParameter =>
                    _dynamicParameterPermissionChecker.IsGranted(entityDynamicParameter.DynamicParameterId))
                .ToList();
            return allParameters;
        }

        public async Task<List<EntityDynamicParameter>> GetAllAsync()
        {
            var allParameters = await EntityDynamicParameterStore.GetAllAsync();

            var controlledParameters = new List<EntityDynamicParameter>();
            foreach (var entityDynamicParameter in allParameters)
            {
                if (await _dynamicParameterPermissionChecker.IsGrantedAsync(entityDynamicParameter.DynamicParameterId))
                {
                    controlledParameters.Add(entityDynamicParameter);
                }
            }
            return controlledParameters;
        }

        public virtual void Add(EntityDynamicParameter entityDynamicParameter)
        {
            CheckEntityName(entityDynamicParameter.EntityFullName);
            _dynamicParameterPermissionChecker.CheckPermission(entityDynamicParameter.DynamicParameterId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                EntityDynamicParameterStore.Add(entityDynamicParameter);
                uow.Complete();
            }

            EntityDynamicParameterCache.Set(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual async Task AddAsync(EntityDynamicParameter entityDynamicParameter)
        {
            CheckEntityName(entityDynamicParameter.EntityFullName);
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(entityDynamicParameter.DynamicParameterId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await EntityDynamicParameterStore.AddAsync(entityDynamicParameter);
                uow.Complete();
            }
            await EntityDynamicParameterCache.SetAsync(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual void Update(EntityDynamicParameter entityDynamicParameter)
        {
            CheckEntityName(entityDynamicParameter.EntityFullName);
            _dynamicParameterPermissionChecker.CheckPermission(entityDynamicParameter.DynamicParameterId);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                EntityDynamicParameterStore.Update(entityDynamicParameter);
                uow.Complete();
            }
            EntityDynamicParameterCache.Set(entityDynamicParameter.Id, entityDynamicParameter);
        }

        public virtual async Task UpdateAsync(EntityDynamicParameter entityDynamicParameter)
        {
            CheckEntityName(entityDynamicParameter.EntityFullName);
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(entityDynamicParameter.DynamicParameterId);

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
