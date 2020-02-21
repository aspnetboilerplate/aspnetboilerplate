using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterValueManager : IEntityDynamicParameterValueManager, ITransientDependency
    {
        private readonly IDynamicParameterPermissionChecker _dynamicParameterPermissionChecker;

        public IEntityDynamicParameterValueStore EntityDynamicParameterValueStore { get; set; }

        public EntityDynamicParameterValueManager(
            IDynamicParameterPermissionChecker dynamicParameterPermissionChecker
            )
        {
            _dynamicParameterPermissionChecker = dynamicParameterPermissionChecker;
            EntityDynamicParameterValueStore = NullEntityDynamicParameterValueStore.Instance;
        }

        public EntityDynamicParameterValue Get(int id)
        {
            return EntityDynamicParameterValueStore.Get(id);
        }

        public Task<EntityDynamicParameterValue> GetAsync(int id)
        {
            return EntityDynamicParameterValueStore.GetAsync(id);
        }

        public EntityDynamicParameterValue Get(string entityId)
        {
            return EntityDynamicParameterValueStore.Get(entityId);
        }

        public Task<EntityDynamicParameterValue> GetAsync(string entityId)
        {
            return EntityDynamicParameterValueStore.GetAsync(entityId);
        }

        public void Add(EntityDynamicParameterValue dynamicParameterValue)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(dynamicParameterValue.EntityDynamicParameterId);
            EntityDynamicParameterValueStore.Add(dynamicParameterValue);
        }

        public async Task AddAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(dynamicParameterValue.EntityDynamicParameterId);
            await EntityDynamicParameterValueStore.AddAsync(dynamicParameterValue);
        }

        public void Update(EntityDynamicParameterValue dynamicParameterValue)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(dynamicParameterValue.EntityDynamicParameterId);
            EntityDynamicParameterValueStore.Update(dynamicParameterValue);
        }

        public async Task UpdateAsync(EntityDynamicParameterValue dynamicParameterValue)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(dynamicParameterValue.EntityDynamicParameterId);
            await EntityDynamicParameterValueStore.UpdateAsync(dynamicParameterValue);
        }

        public void Delete(int id)
        {
            var dynamicParameterValue = Get(id);

            _dynamicParameterPermissionChecker.CheckPermissions(dynamicParameterValue.EntityDynamicParameterId);
            EntityDynamicParameterValueStore.Delete(id);
        }

        public async Task DeleteAsync(int id)
        {
            var dynamicParameterValue = await GetAsync(id);

            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(dynamicParameterValue.EntityDynamicParameterId);
            await EntityDynamicParameterValueStore.DeleteAsync(id);
        }

        public List<string> GetAllPossibleValueOfDynamicParameter(int dynamicParameterId)
        {
            return EntityDynamicParameterValueStore.GetAllPossibleValueOfDynamicParameter(dynamicParameterId);
        }

        public Task<List<string>> GetAllPossibleValueOfDynamicParameterAsync(int dynamicParameterId)
        {
            return EntityDynamicParameterValueStore.GetAllPossibleValueOfDynamicParameterAsync(dynamicParameterId);
        }
    }
}
