using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.RealTime
{
    /// <summary>
    /// Implements <see cref="IOnlineClientManager"/>.
    /// </summary>
    public class OnlineClientManager : IOnlineClientManager, ISingletonDependency
    {
        /// <summary>
        /// Reference to Logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Online clients.
        /// </summary>
        private readonly ConcurrentDictionary<string, IOnlineClient> _clients;

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
        /// </summary>
        public OnlineClientManager()
        {
            _clients = new ConcurrentDictionary<string, IOnlineClient>();

            Logger = NullLogger.Instance;
        }

        public void Add(IOnlineClient client)
        {
            Logger.Debug("Connected: " + client);
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

        public IOnlineClient GetByUserIdOrNull(long userId)
        {
            return GetAllClients().FirstOrDefault(c => c.UserId == userId); //TODO: We can create an index for a faster lookup.
        }

        public IReadOnlyList<IOnlineClient> GetAllClients()
        {
            return _clients.Values.ToImmutableList();
        }
    }
}