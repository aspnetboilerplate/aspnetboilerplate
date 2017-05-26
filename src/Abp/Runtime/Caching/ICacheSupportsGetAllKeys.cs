using System;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    /// <summary>
    /// Defines a cache that support method to return cache keys.
    /// </summary>
    public interface ICacheSupportsGetAllKeys : IDisposable
    {
        /// <summary>
        /// Returns all keys of cache.
        /// </summary>
        /// <returns>String array of keys</returns>
        string[] GetAllKeys();
    }
}