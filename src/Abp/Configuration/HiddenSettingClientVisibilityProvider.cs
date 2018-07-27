using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Configuration
{
    public class HiddenSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisible(IScopedIocResolver scope)
        {
            return await Task.FromResult(false);
        }
    }
}