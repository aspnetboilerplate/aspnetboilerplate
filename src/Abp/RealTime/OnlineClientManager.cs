using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;

namespace Abp.RealTime
{
    /// <summary>
    ///     Implements <see cref="IOnlineClientManager" />.
    /// </summary>
    public class OnlineClientManager : IOnlineClientManager, ISingletonDependency
    {
        /// <summary>
        ///     Online clients.
        /// </summary>
        private readonly ConcurrentDictionary<string, IOnlineClient> _clients;

        /// <summary>
        ///     Initializes a new instance of the <see cref="OnlineClientManager" /> class.
        /// </summary>
        public OnlineClientManager()
        {
            _clients = new ConcurrentDictionary<string, IOnlineClient>();
        }

        public void Add(IOnlineClient client)
        {
            _clients[client.ConnectionId] = client;
        }

        public bool Remove(IOnlineClient client)
        {
            return Remove(client.ConnectionId);
        }

        public bool Remove(string connectionId)
        {
            IOnlineClient client;
            return _clients.TryRemove(connectionId, out client);
        }

        public IOnlineClient GetByConnectionIdOrNull(string connectionId)
        {
            return _clients.GetOrDefault(connectionId);
        }

        public IOnlineClient GetByUserIdOrNull(IUserIdentifier user)
        {
            //TODO: We can create a dictionary for a faster lookup.
            return GetAllClients().FirstOrDefault(c => c.UserId == user.UserId && c.TenantId == user.TenantId);
        }

        public IReadOnlyList<IOnlineClient> GetAllClients()
        {
            return _clients.Values.ToImmutableList();
        }
    }
}