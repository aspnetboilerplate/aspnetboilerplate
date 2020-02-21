using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicParameterPermissionChecker
    {
        void CheckPermissions(int dynamicParameterId);

        Task CheckPermissionsAsync(int dynamicParameterId);
    }
}
