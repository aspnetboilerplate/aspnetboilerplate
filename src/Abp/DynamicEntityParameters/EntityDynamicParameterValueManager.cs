using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterValueManager : IEntityDynamicParameterValueManager, ITransientDependency
    {
        private readonly IDynamicParameterPermissionChecker _dynamicParameterPermissionChecker;
        private readonly IDynamicParameterManager _dynamicParameterManager;
        private readonly IEntityDynamicParameterManager _entityDynamicParameterManager;

        public IEntityDynamicParameterValueStore EntityDynamicParameterValueStore { get; set; }

        public EntityDynamicParameterValueManager(
            IDynamicParameterPermissionChecker dynamicParameterPermissionChecker,
            IDynamicParameterManager dynamicParameterManager,
            IEntityDynamicParameterManager entityDynamicParameterManager
            )
        {
            _dynamicParameterPermissionChecker = dynamicParameterPermissionChecker;
            _dynamicParameterManager = dynamicParameterManager;
            _entityDynamicParameterManager = entityDynamicParameterManager;
            EntityDynamicParameterValueStore = NullEntityDynamicParameterValueStore.Instance;
        }

        private int GetDynamicParameterId(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            if (entityDynamicParameterValue.EntityDynamicParameterId == default)
            {
                throw new ArgumentNullException(nameof(entityDynamicParameterValue.EntityDynamicParameterId));
            }

            return entityDynamicParameterValue.EntityDynamicParameter?.DynamicParameterId ??
                   _dynamicParameterManager.Get(entityDynamicParameterValue.EntityDynamicParameterId).Id;
        }

        private async Task<int> GetDynamicParameterIdAsync(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            if (entityDynamicParameterValue.EntityDynamicParameterId == default)
            {
                throw new ArgumentNullException(nameof(entityDynamicParameterValue.EntityDynamicParameterId));
            }

            return entityDynamicParameterValue.EntityDynamicParameter?.DynamicParameterId ??
                 (await _dynamicParameterManager.GetAsync(entityDynamicParameterValue.EntityDynamicParameterId)).Id;
        }

        public virtual EntityDynamicParameterValue Get(int id)
        {
            var value = EntityDynamicParameterValueStore.Get(id);
            _dynamicParameterPermissionChecker.CheckPermission(GetDynamicParameterId(value));
            return value;
        }

        public virtual async Task<EntityDynamicParameterValue> GetAsync(int id)
        {
            var value = await EntityDynamicParameterValueStore.GetAsync(id);
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(await GetDynamicParameterIdAsync(value));
            return value;
        }

        public virtual void Add(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            _dynamicParameterPermissionChecker.CheckPermission(GetDynamicParameterId(entityDynamicParameterValue));
            EntityDynamicParameterValueStore.Add(entityDynamicParameterValue);
        }

        public virtual async Task AddAsync(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(await GetDynamicParameterIdAsync(entityDynamicParameterValue));
            await EntityDynamicParameterValueStore.AddAsync(entityDynamicParameterValue);
        }

        public virtual void Update(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            _dynamicParameterPermissionChecker.CheckPermission(GetDynamicParameterId(entityDynamicParameterValue));
            EntityDynamicParameterValueStore.Update(entityDynamicParameterValue);
        }

        public virtual async Task UpdateAsync(EntityDynamicParameterValue entityDynamicParameterValue)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(await GetDynamicParameterIdAsync(entityDynamicParameterValue));
            await EntityDynamicParameterValueStore.UpdateAsync(entityDynamicParameterValue);
        }

        public virtual void Delete(int id)
        {
            var entityDynamicParameterValue = Get(id);//Get checks permission, no need to check it again
            if (entityDynamicParameterValue == null)
            {
                return;
            }
            EntityDynamicParameterValueStore.Delete(id);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var entityDynamicParameterValue = await GetAsync(id);//Get checks permission, no need to check it again
            if (entityDynamicParameterValue == null)
            {
                return;
            }
            await EntityDynamicParameterValueStore.DeleteAsync(id);
        }

        public List<EntityDynamicParameterValue> GetValues(int entityDynamicParameterId, string entityRowId)
        {
            var entityDynamicParameter = _entityDynamicParameterManager.Get(entityDynamicParameterId);
            _dynamicParameterPermissionChecker.CheckPermission(entityDynamicParameter.DynamicParameterId);

            return EntityDynamicParameterValueStore.GetValues(entityDynamicParameterId, entityRowId);
        }

        public async Task<List<EntityDynamicParameterValue>> GetValuesAsync(int entityDynamicParameterId, string entityRowId)
        {
            var entityDynamicParameter = await _entityDynamicParameterManager.GetAsync(entityDynamicParameterId);
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(entityDynamicParameter.DynamicParameterId);

            return await EntityDynamicParameterValueStore.GetValuesAsync(entityDynamicParameterId, entityRowId);
        }

        public List<EntityDynamicParameterValue> GetValues(string entityFullName, string entityRowId)
        {
            return EntityDynamicParameterValueStore.GetValues(entityFullName, entityRowId)
                 .Where(value =>
                 {
                     var entityDynamicParameter = _entityDynamicParameterManager.Get(value.EntityDynamicParameterId);
                     return _dynamicParameterPermissionChecker.IsGranted(entityDynamicParameter.DynamicParameterId);
                 })
                 .ToList();
        }

        public async Task<List<EntityDynamicParameterValue>> GetValuesAsync(string entityFullName, string entityRowId)
        {
            var allValues = await EntityDynamicParameterValueStore.GetValuesAsync(entityFullName, entityRowId);
            var returnList = new List<EntityDynamicParameterValue>();

            foreach (var value in allValues)
            {
                var entityDynamicParameter = _entityDynamicParameterManager.Get(value.EntityDynamicParameterId);

                if (await _dynamicParameterPermissionChecker.IsGrantedAsync(entityDynamicParameter.DynamicParameterId))
                {
                    returnList.Add(value);
                }
            }

            return returnList;
        }

        public void CleanValues(int entityDynamicParameterId, string entityRowId)
        {
            var entityDynamicParameter = _entityDynamicParameterManager.Get(entityDynamicParameterId);
            _dynamicParameterPermissionChecker.CheckPermission(entityDynamicParameter.DynamicParameterId);

            EntityDynamicParameterValueStore.GetValues(entityDynamicParameterId, entityRowId);
        }

        public async Task CleanValuesAsync(int entityDynamicParameterId, string entityRowId)
        {
            var entityDynamicParameter = await _entityDynamicParameterManager.GetAsync(entityDynamicParameterId);
            await _dynamicParameterPermissionChecker.CheckPermissionAsync(entityDynamicParameter.DynamicParameterId);

            await EntityDynamicParameterValueStore.CleanValuesAsync(entityDynamicParameterId, entityRowId);
        }
    }
}
