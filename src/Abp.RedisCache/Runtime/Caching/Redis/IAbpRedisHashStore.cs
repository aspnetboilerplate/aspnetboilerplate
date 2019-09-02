using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using JetBrains.Annotations;

namespace Abp.Runtime.Caching.Redis
{
    public interface IAbpRedisHashStore<TKey, TValue>
    {
        string StoreName { get; }
        long Count { get; }
       
        void Add([NotNull] TKey key, TValue value);
        bool Update([NotNull] TKey key, TValue value);
        void Clear();
        bool ContainsKey([NotNull] TKey key);
        bool Remove([NotNull] TKey key);
        bool TryAdd([NotNull] TKey key, TValue value);
        bool TryGetValue([NotNull] TKey key, out TValue value);

        IImmutableList<TKey> GetAllKeys();
        IImmutableList<TValue> GetAllValues();
    }
}
