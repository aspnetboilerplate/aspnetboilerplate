using System.Threading.Tasks;

namespace Abp.Domain.Entities.Caching
{
    public interface IEntityCacheBase<TCacheItem, TPrimaryKey>
    {
        TCacheItem this[TPrimaryKey id] { get; }

        string CacheName { get; }

        TCacheItem Get(TPrimaryKey id);

        Task<TCacheItem> GetAsync(TPrimaryKey id);
    }
}