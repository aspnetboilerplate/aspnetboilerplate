using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Dependency;
using JetBrains.Annotations;

namespace Abp.RealTime
{
    public class OnlineClientManager<T> : OnlineClientManager, IOnlineClientManager<T>
    {
        public OnlineClientManager(IOnlineClientStore<T> store) : base(store)
        {

        }
    }

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
        /// Online clients Store.
        /// </summary>
        protected IOnlineClientStore Store { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OnlineClientManager"/> class.
        /// </summary>
        public OnlineClientManager(IOnlineClientStore store)
        {
            Store = store;
        }

        public virtual void Add(IOnlineClient client)
        {
            var userWasAlreadyOnline = false;
            var user = client.ToUserIdentifierOrNull();

            if (user != null)
            {
                userWasAlreadyOnline = this.IsOnline(user);
            }

            Store.Add(client);

            ClientConnected?.Invoke(this, new OnlineClientEventArgs(client));

            if (user != null && !userWasAlreadyOnline)
            {
                UserConnected?.Invoke(this, new OnlineUserEventArgs(user, client));
            }
        }

        public virtual bool Remove(string connectionId)
        {
            var result = Store.TryRemove(connectionId, out IOnlineClient client);
            if (!result)
            {
                return false;
            }
            
            if (UserDisconnected != null)
            {
                var user = client.ToUserIdentifierOrNull();

                if (user != null && !this.IsOnline(user))
                {
                    UserDisconnected.Invoke(this, new OnlineUserEventArgs(user, client));
                }
            }

            ClientDisconnected?.Invoke(this, new OnlineClientEventArgs(client));

            return true;
        }

        public virtual IOnlineClient GetByConnectionIdOrNull(string connectionId)
        {
            if (Store.TryGet(connectionId, out IOnlineClient client))
            {
                return client;
            }

            return null;
        }
        
        public virtual IReadOnlyList<IOnlineClient> GetAllClients()
        {
            return Store.GetAll();
        }

        [NotNull]
        public virtual IReadOnlyList<IOnlineClient> GetAllByUserId([NotNull] IUserIdentifier user)
        {
            Check.NotNull(user, nameof(user));

            return GetAllClients()
                 .Where(c => c.UserId == user.UserId && c.TenantId == user.TenantId)
                 .ToImmutableList();
        }
    }
}
