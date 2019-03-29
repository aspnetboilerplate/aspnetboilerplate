using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Data;

namespace Abp.RealTime
{
    public class InMemoryOnlineClientStore : IOnlineClientStore
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

        public ConditionalValue<IOnlineClient> Remove(string connectionId)
        {
           return Clients.TryRemove(connectionId, out IOnlineClient removed) ? new ConditionalValue<IOnlineClient>(true, removed) : new ConditionalValue<IOnlineClient>(false, null);
        }

        public ConditionalValue<IOnlineClient> Get(string connectionId)
        {
            return Clients.TryGetValue(connectionId, out IOnlineClient found) ? new ConditionalValue<IOnlineClient>(true, found) : new ConditionalValue<IOnlineClient>(false, null);
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
