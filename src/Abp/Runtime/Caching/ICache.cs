using System;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Defines a cache that uses <see cref="string"/> as key, <see cref="object"/> as value.
    /// </summary>
    public interface ICache : IDisposable, ICacheOptions, IAbpCache<string, object>
    {
    }
}
