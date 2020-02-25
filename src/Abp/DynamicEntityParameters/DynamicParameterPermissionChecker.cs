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
        private readonly IDynamicParameterManager _dynamicParameterManager;

        public DynamicParameterPermissionChecker(
            IPermissionChecker permissionChecker,
            IDynamicParameterManager dynamicParameterManager
            )
        {
            _permissionChecker = permissionChecker;
            _dynamicParameterManager = dynamicParameterManager;
        }

        public void CheckPermissions(int dynamicParameterId)
        {
            var dynamicParameter = _dynamicParameterManager.Get(dynamicParameterId);
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
            var dynamicParameter = await _dynamicParameterManager.GetAsync(dynamicParameterId);
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
