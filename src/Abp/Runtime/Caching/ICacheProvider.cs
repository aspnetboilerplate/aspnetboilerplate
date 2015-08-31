using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheProvider
    {
        ICacheStore<TKey, TValue> GetCacheStore<TKey, TValue>(string name);

        Task<ICacheStore<TKey, TValue>> GetCacheStoreAsync<TKey, TValue>(string name);
    }
}
