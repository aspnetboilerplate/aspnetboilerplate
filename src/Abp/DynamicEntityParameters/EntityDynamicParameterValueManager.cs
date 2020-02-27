using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterValueManager : IEntityDynamicParameterValueManager, ITransientDependency
    {
        private readonly IDynamicParameterPermissionChecker _dynamicParameterPermissionChecker;
        private readonly IDynamicParameterManager _dynamicParameterManager;

        public IEntityDynamicParameterValueStore EntityDynamicParameterValueStore { get; set; }

        public EntityDynamicParameterValueManager(
            IDynamicParameterPermissionChecker dynamicParameterPermissionChecker,
            IDynamicParameterManager dynamicParameterManager
            )
        {
            _dynamicParameterPermissionChecker = dynamicParameterPermissionChecker;
            _dynamicParameterManager = dynamicParameterManager;
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
            _dynamicParameterPermissionChecker.CheckPermissions(GetDynamicParameterId(value));
            return value;
        }

        public virtual async Task<EntityDynamicParameterValue> GetAsync(int id)
        {
            var value = await EntityDynamicParameterValueStore.GetAsync(id);
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(await GetDynamicParameterIdAsync(value));
            return value;
        }

        public virtual void Add(EntityDynamicParameterValue dynamicParameterValue)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(GetDynamicParameterId(dynamicParameterValue));
            EntityDynamicParameterValueStore.Add(dynamicParameterValue);
        }

        public virtual async Task AddAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(await GetDynamicParameterIdAsync(dynamicParameterValue));
            await EntityDynamicParameterValueStore.AddAsync(dynamicParameterValue);
        }

        public virtual void Update(EntityDynamicParameterValue dynamicParameterValue)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(GetDynamicParameterId(dynamicParameterValue));
            EntityDynamicParameterValueStore.Update(dynamicParameterValue);
        }

        public virtual async Task UpdateAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(await GetDynamicParameterIdAsync(dynamicParameterValue));
            await EntityDynamicParameterValueStore.UpdateAsync(dynamicParameterValue);
        }

        public virtual void Delete(int id)
        {
            var dynamicParameterValue = Get(id);//Get checks permission, no need to check it again
            if (dynamicParameterValue == null)
            {
                return;
            }
            EntityDynamicParameterValueStore.Delete(id);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var dynamicParameterValue = await GetAsync(id);//Get checks permission, no need to check it again
            if (dynamicParameterValue == null)
            {
                return;
            }
            await EntityDynamicParameterValueStore.DeleteAsync(id);
        }
    }
}
