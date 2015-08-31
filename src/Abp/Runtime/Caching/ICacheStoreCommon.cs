using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheStoreCommon : IDisposable
    {
        string Name { get; }

        TimeSpan DefaultSlidingExpireTime { get; set; }

        void Clear();

        Task ClearAsync();
    }
}