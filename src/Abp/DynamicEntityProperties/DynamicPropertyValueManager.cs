using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.DynamicEntityProperties
{
    public class DynamicPropertyValueManager : IDynamicPropertyValueManager, ITransientDependency
    {
        private readonly IDynamicPropertyPermissionChecker _dynamicPropertyPermissionChecker;

        public IDynamicPropertyValueStore DynamicPropertyValueStore { get; set; }

        public DynamicPropertyValueManager(IDynamicPropertyPermissionChecker dynamicPropertyPermissionChecker)
        {
            _dynamicPropertyPermissionChecker = dynamicPropertyPermissionChecker;
            DynamicPropertyValueStore = NullDynamicPropertyValueStore.Instance;
        }

        public virtual DynamicPropertyValue Get(long id)
        {
            var val = DynamicPropertyValueStore.Get(id);
            _dynamicPropertyPermissionChecker.CheckPermission(val.DynamicPropertyId);
            return val;
        }

        public virtual async Task<DynamicPropertyValue> GetAsync(long id)
        {
            var val = await DynamicPropertyValueStore.GetAsync(id);
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(val.DynamicPropertyId);
            return val;
        }

        public virtual List<DynamicPropertyValue> GetAllValuesOfDynamicProperty(int dynamicPropertyId)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyId);
            return DynamicPropertyValueStore.GetAllValuesOfDynamicProperty(dynamicPropertyId);
        }

        public virtual async Task<List<DynamicPropertyValue>> GetAllValuesOfDynamicPropertyAsync(int dynamicPropertyId)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyId);
            return await DynamicPropertyValueStore.GetAllValuesOfDynamicPropertyAsync(dynamicPropertyId);
        }

        public virtual void Add(DynamicPropertyValue dynamicPropertyValue)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyValue.DynamicPropertyId);
            DynamicPropertyValueStore.Add(dynamicPropertyValue);
        }

        public virtual async Task AddAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyValue.DynamicPropertyId);
            await DynamicPropertyValueStore.AddAsync(dynamicPropertyValue);
        }

        public virtual void Update(DynamicPropertyValue dynamicPropertyValue)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyValue.DynamicPropertyId);
            DynamicPropertyValueStore.Update(dynamicPropertyValue);
        }

        public virtual async Task UpdateAsync(DynamicPropertyValue dynamicPropertyValue)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyValue.DynamicPropertyId);
            await DynamicPropertyValueStore.UpdateAsync(dynamicPropertyValue);
        }

        public virtual void Delete(long id)
        {
            var val = Get(id);
            if (val != null)//Get checks permission, no need to check it again  
            {
                DynamicPropertyValueStore.Delete(id);
            }
        }

        public virtual async Task DeleteAsync(long id)
        {
            var val = await GetAsync(id);
            if (val != null)//Get checks permission, no need to check it again
            {
                await DynamicPropertyValueStore.DeleteAsync(id);
            }
        }

        public virtual void CleanValues(int dynamicPropertyId)
        {
            _dynamicPropertyPermissionChecker.CheckPermission(dynamicPropertyId);
            DynamicPropertyValueStore.CleanValues(dynamicPropertyId);
        }

        public virtual async Task CleanValuesAsync(int dynamicPropertyId)
        {
            await _dynamicPropertyPermissionChecker.CheckPermissionAsync(dynamicPropertyId);
            await DynamicPropertyValueStore.CleanValuesAsync(dynamicPropertyId);
        }
    }
}
