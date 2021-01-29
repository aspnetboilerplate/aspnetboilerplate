using System.Threading.Tasks;

namespace Abp.Caching
{
    public interface IGetScriptsResponsePerUserCache
    {
        Task<string> GetKey();

        Task RemoveKey();

        Task ClearAll();
    }
}