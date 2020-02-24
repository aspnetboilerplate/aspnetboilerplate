using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;

namespace Abp.DynamicEntityParameters
{
    public class DynamicParameterPermissionChecker : IDynamicParameterPermissionChecker, ITransientDependency
    {
        private readonly IPermissionChecker _permissionChecker;
        public IDynamicParameterStore DynamicParameterStore { get; set; }

        public DynamicParameterPermissionChecker(
            IPermissionChecker permissionChecker
            )
        {
            _permissionChecker = permissionChecker;
            DynamicParameterStore = NullDynamicParameterStore.Instance;
        }

        public void CheckPermissions(int dynamicParameterId)
        {
            var dynamicParameter = DynamicParameterStore.Get(dynamicParameterId);
            if (dynamicParameter == null)
            {
                throw new EntityNotFoundException(typeof(DynamicParameter), dynamicParameterId);
            }

            if (!_permissionChecker.IsGranted(dynamicParameter.Permission))
            {
                throw new Exception($"Permission \"{dynamicParameter.Permission}\" is not granted");
            }
        }

        public async Task CheckPermissionsAsync(int dynamicParameterId)
        {
            var dynamicParameter = await DynamicParameterStore.GetAsync(dynamicParameterId);
            if (dynamicParameter == null)
            {
                throw new EntityNotFoundException(typeof(DynamicParameter), dynamicParameterId);
            }

            if (!await _permissionChecker.IsGrantedAsync(dynamicParameter.Permission))
            {
                throw new Exception($"Permission \"{dynamicParameter.Permission}\" is not granted");
            }
        }
    }
}
