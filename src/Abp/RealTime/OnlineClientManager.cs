using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Extensions;

namespace Abp.RealTime
{
    /// <summary>
    /// Implements <see cref="IOnlineClientManager"/>.
    /// </summary>
    public class OnlineClientManager : IOnlineClientManager, ISingletonDependency
    {
        public event EventHandler<OnlineClientEventArgs> ClientConnected;
        public event EventHandler<OnlineClientEventArgs> ClientDisconnected;
        public event EventHandler<OnlineUserEventArgs> UserConnected;
        public event EventHandler<OnlineUserEventArgs> UserDisconnected;

        /// <summary>
        /// Online clients.
        /// </summary>
        private readonly ConcurrentDictionary<string, IOnlineClient> _clients;

        private readonly object _syncObj = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
        /// </summary>
        public OnlineClientManager()
        {
            _clients = new ConcurrentDictionary<string, IOnlineClient>();
        }

        public void Add(IOnlineClient client)
        {
            lock (_syncObj)
            {
                var userWasAlreadyOnline = false;
                var user = client.ToUserIdentifierOrNull();

                if (user != null)
                {
                    userWasAlreadyOnline = this.IsOnline(user);
                }

                _clients[client.ConnectionId] = client;

                ClientConnected.InvokeSafely(this, new OnlineClientEventArgs(client));

                if (user != null && !userWasAlreadyOnline)
                {
                    UserConnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
                }
            }
        }

        public bool Remove(string connectionId)
        {
            lock (_syncObj)
            {
                IOnlineClient client;
                var isRemoved = _clients.TryRemove(connectionId, out client);

                if (isRemoved)
                {
                    var user = client.ToUserIdentifierOrNull();

                    if (user != null && !this.IsOnline(user))
                    {
                        UserDisconnected.InvokeSafely(this, new OnlineUserEventArgs(user, client));
                    }

                    ClientDisconnected.InvokeSafely(this, new OnlineClientEventArgs(client));
                }

                return isRemoved;
            }
        }

        public IOnlineClient GetByConnectionIdOrNull(string connectionId)
        {
            lock (_syncObj)
            {
                return _clients.GetOrDefault(connectionId);
            }
        }
        
        public IReadOnlyList<IOnlineClient> GetAllClients()
        {
            lock (_syncObj)
            {
                return _clients.Values.ToImmutableList();
            }
        }
    }
}