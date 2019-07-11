using System;
using System.Collections.Generic;
using System.Text;

namespace Abp.Runtime.Caching.Redis
{
    public interface IAbpRedisHashStoreProvider
    {
        IAbpRedisHashStore<TKey, TValue> GetAbpRedisHashStore<TKey, TValue>(string storeName);
    }
}
