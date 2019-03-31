using System.Collections.Immutable;
using JetBrains.Annotations;

namespace Abp.RealTime.Redis
{
    public interface IRedisKeyValueStore<TKey, TValue>
    {
        int Count { get; }
        IImmutableList<TKey> Keys { get; }
        IImmutableList<TValue> Values { get; }

        void Add([NotNull] TKey key, TValue value);
        bool Update([NotNull] TKey key, TValue value);
        void Clear();
        bool ContainsKey([NotNull] TKey key);
        bool Remove([NotNull] TKey key);
        bool TryAdd([NotNull] TKey key, TValue value);
        bool TryGetValue([NotNull] TKey key, out TValue value);
    }
}