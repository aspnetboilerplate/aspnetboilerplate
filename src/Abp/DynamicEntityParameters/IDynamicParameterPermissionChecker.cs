using System.Threading.Tasks;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicParameterPermissionChecker
    {
        void CheckPermission(int dynamicParameterId);

        Task CheckPermissionAsync(int dynamicParameterId);

        bool IsGranted(int dynamicParameterId);

        Task<bool> IsGrantedAsync(int dynamicParameterId);
    }
}
