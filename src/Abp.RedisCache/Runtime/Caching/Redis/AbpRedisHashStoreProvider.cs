using System;
using System.Collections.Generic;
using System.Text;
using Abp.Dependency;

namespace Abp.Runtime.Caching.Redis
{
    public class AbpRedisHashStoreProvider : IAbpRedisHashStoreProvider, ISingletonDependency
    {
        private readonly IAbpRedisCacheDatabaseProvider _databaseProvider;
        private readonly IRedisCacheSerializer _serializer;

        public AbpRedisHashStoreProvider(IAbpRedisCacheDatabaseProvider databaseProvider, IRedisCacheSerializer serializer)
        {
            _databaseProvider = databaseProvider;
            _serializer = serializer;
        }
        public IAbpRedisHashStore<TKey, TValue> GetAbpRedisHashStore<TKey, TValue>(string storeName)
        {
            return new AbpRedisHashStore<TKey, TValue>(storeName, _databaseProvider, _serializer);
        }
    }
}
