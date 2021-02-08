using System.Threading.Tasks;

namespace Abp.CachedUniqueKeys
{
    public interface ICachedUniqueKeyPerUser
    {
        Task<string> GetKeyAsync(string cacheName);

        Task RemoveKeyAsync(string cacheName);

        Task ClearCacheAsync(string cacheName);

        string GetKey(string cacheName);

        void RemoveKey(string cacheName);

        void ClearCache(string cacheName);
    }
}