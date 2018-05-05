using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Configuration
{
    public class RequiresAuthenticationSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisible(IScopedIocResolver scope)
        {
            return await Task.FromResult(
                scope.Resolve<IAbpSession>().UserId.HasValue
            );
        }
    }
}