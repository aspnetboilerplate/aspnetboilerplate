using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Dependency;

namespace Abp.RealTime
{
    public class InMemoryOnlineClientStore<T> : InMemoryOnlineClientStore, IOnlineClientStore<T>
    {
    }

    public class InMemoryOnlineClientStore : IOnlineClientStore, ISingletonDependency
    {
        /// <summary>
        /// Online clients.
        /// </summary>
        protected ConcurrentDictionary<string, IOnlineClient> Clients { get; }

        public InMemoryOnlineClientStore()
        {
            Clients = new ConcurrentDictionary<string, IOnlineClient>();
        }

        public void Add(IOnlineClient client)
        {
            Clients.AddOrUpdate(client.ConnectionId, client, (s, o) => client);
        }

        public bool Remove(string connectionId)
        {
            return TryRemove(connectionId, out IOnlineClient removed);
        }

        public bool TryRemove(string connectionId, out IOnlineClient client)
        {
            return Clients.TryRemove(connectionId, out client);
        }

        public bool TryGet(string connectionId, out IOnlineClient client)
        {
            return Clients.TryGetValue(connectionId, out client);
        }

        public bool Contains(string connectionId)
        {
            return Clients.ContainsKey(connectionId);
        }

        public IReadOnlyList<IOnlineClient> GetAll()
        {
            return Clients.Values.ToImmutableList();
        }
    }
}
