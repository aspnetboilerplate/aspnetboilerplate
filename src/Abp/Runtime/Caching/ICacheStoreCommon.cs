using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheStoreCommon : IDisposable //TODO: Think a better naming!
    {
        string Name { get; }

        TimeSpan DefaultSlidingExpireTime { get; set; }

        void Clear();

        Task ClearAsync();
    }
}