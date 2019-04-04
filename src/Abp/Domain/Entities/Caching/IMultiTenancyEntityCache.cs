using System.Threading.Tasks;
using Abp.Runtime.Caching;

namespace Abp.Domain.Entities.Caching
{
    public interface IMultiTenanyEntityCache<TCacheItem> : IMultiTenanyEntityCache<TCacheItem, int>
    {

    }

    public interface IMultiTenanyEntityCache<TCacheItem, TPrimaryKey>
    {
        TCacheItem this[TPrimaryKey id] { get; }

        string CacheName { get; }

        ITypedCache<string, TCacheItem> InternalCache { get; }

        string GetCacheKey(TPrimaryKey id);

        string GetCacheKey(TPrimaryKey id, int? tenantId);

        TCacheItem Get(TPrimaryKey id);

        Task<TCacheItem> GetAsync(TPrimaryKey id);
    }
}