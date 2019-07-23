using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Runtime.Caching
{
    public interface ICacheOperations
    {
        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        void Clear();

        /// <summary>
        /// Clears all items in this cache.
        /// </summary>
        Task ClearAsync();
    }
}
