using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.DynamicEntityParameters
{
    public class EntityDynamicParameterManager : IEntityDynamicParameterManager, ITransientDependency
    {
        private readonly IDynamicParameterPermissionChecker _dynamicParameterPermissionChecker;

        public IEntityDynamicParameterStore EntityDynamicParameterStore { get; set; }

        public EntityDynamicParameterManager(
            IDynamicParameterPermissionChecker dynamicParameterPermissionChecker
            )
        {
            _dynamicParameterPermissionChecker = dynamicParameterPermissionChecker;
            EntityDynamicParameterStore = NullEntityDynamicParameterStore.Instance;
        }

        public EntityDynamicParameter Get(int id)
        {
            return EntityDynamicParameterStore.Get(id);
        }

        public Task<EntityDynamicParameter> GetAsync(int id)
        {
            return EntityDynamicParameterStore.GetAsync(id);
        }

        public void Add(EntityDynamicParameter entityDynamicParameter)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(entityDynamicParameter.DynamicParameterId);
            EntityDynamicParameterStore.Add(entityDynamicParameter);
        }

        public async Task AddAsync(EntityDynamicParameter entityDynamicParameter)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(entityDynamicParameter.DynamicParameterId);
            await EntityDynamicParameterStore.AddAsync(entityDynamicParameter);
        }

        public void Update(EntityDynamicParameter entityDynamicParameter)
        {
            _dynamicParameterPermissionChecker.CheckPermissions(entityDynamicParameter.DynamicParameterId);
            EntityDynamicParameterStore.Update(entityDynamicParameter);
        }

        public async Task UpdateAsync(EntityDynamicParameter entityDynamicParameter)
        {
            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(entityDynamicParameter.DynamicParameterId);
            await EntityDynamicParameterStore.UpdateAsync(entityDynamicParameter);
        }

        public void Delete(int id)
        {
            var entityDynamicParameter = Get(id);
            if (entityDynamicParameter == null)
            {
                return;
            }

            _dynamicParameterPermissionChecker.CheckPermissions(entityDynamicParameter.DynamicParameterId);
            EntityDynamicParameterStore.Delete(entityDynamicParameter.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var entityDynamicParameter = await GetAsync(id);
            if (entityDynamicParameter == null)
            {
                return;
            }

            await _dynamicParameterPermissionChecker.CheckPermissionsAsync(entityDynamicParameter.DynamicParameterId);
            await EntityDynamicParameterStore.DeleteAsync(entityDynamicParameter.Id);
        }
    }
}
