using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.Configuration
{
    public interface ISettingClientVisibilityProvider
    {
        Task<bool> CheckVisible(IScopedIocResolver scope);
    }
}