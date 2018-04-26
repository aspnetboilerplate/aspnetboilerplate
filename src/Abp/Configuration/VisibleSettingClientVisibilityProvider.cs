using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Configuration
{
    public class VisibleSettingClientVisibilityProvider : ISettingClientVisibilityProvider
    {
        public async Task<bool> CheckVisible(IScopedIocResolver scope)
        {
            return await Task.FromResult(true);
        }
    }
}