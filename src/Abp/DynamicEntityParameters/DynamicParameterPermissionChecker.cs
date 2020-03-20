using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Extensions;

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

        public void CheckPermission(int dynamicParameterId)
        {
            var dynamicParameter = _dynamicParameterManager.Get(dynamicParameterId);
            if (dynamicParameter == null)
            {
                throw new EntityNotFoundException(typeof(DynamicParameter), dynamicParameterId);
            }

            if (dynamicParameter.Permission.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!_permissionChecker.IsGranted(dynamicParameter.Permission))
            {
                throw new Exception($"Permission \"{dynamicParameter.Permission}\" is not granted");
            }
        }

        public async Task CheckPermissionAsync(int dynamicParameterId)
        {
            var dynamicParameter = await _dynamicParameterManager.GetAsync(dynamicParameterId);
            if (dynamicParameter == null)
            {
                throw new EntityNotFoundException(typeof(DynamicParameter), dynamicParameterId);
            }

            if (dynamicParameter.Permission.IsNullOrWhiteSpace())
            {
                return;
            }

            if (!await _permissionChecker.IsGrantedAsync(dynamicParameter.Permission))
            {
                throw new Exception($"Permission \"{dynamicParameter.Permission}\" is not granted");
            }
        }

        public bool IsGranted(int dynamicParameterId)
        {
            var dynamicParameter = _dynamicParameterManager.Get(dynamicParameterId);
            if (dynamicParameter == null)
            {
                throw new EntityNotFoundException(typeof(DynamicParameter), dynamicParameterId);
            }

            if (dynamicParameter.Permission.IsNullOrWhiteSpace())
            {
                return true;
            }

            return _permissionChecker.IsGranted(dynamicParameter.Permission);
        }

        public async Task<bool> IsGrantedAsync(int dynamicParameterId)
        {
            var dynamicParameter = await _dynamicParameterManager.GetAsync(dynamicParameterId);
            if (dynamicParameter == null)
            {
                throw new EntityNotFoundException(typeof(DynamicParameter), dynamicParameterId);
            }

            if (dynamicParameter.Permission.IsNullOrWhiteSpace())
            {
                return true;
            }

            return await _permissionChecker.IsGrantedAsync(dynamicParameter.Permission);
        }
    }
}
