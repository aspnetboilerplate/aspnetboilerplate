using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Abp.RealTime
{
    /// <summary>
    /// Used to manage online clients those are connected to the application.
    /// </summary>
    public interface IOnlineClientManager<T> : IOnlineClientManager
    {

    }

    public interface IOnlineClientManager
    {
        event EventHandler<OnlineClientEventArgs> ClientConnected;

        event EventHandler<OnlineClientEventArgs> ClientDisconnected;

        event EventHandler<OnlineUserEventArgs> UserConnected;

        event EventHandler<OnlineUserEventArgs> UserDisconnected;

        /// <summary>
        /// Adds a client.
        /// </summary>
        /// <param name="client">The client.</param>
        void Add(IOnlineClient client);

        /// <summary>
        /// Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>True, if a client is removed</returns>
        bool Remove(string connectionId);

        /// <summary>
        /// Tries to find a client by connection id.
        /// Returns null if not found.
        /// </summary>
        /// <param name="connectionId">connection id</param>
        IOnlineClient GetByConnectionIdOrNull(string connectionId);

        /// <summary>
        /// Gets all online clients.
        /// </summary>
        IReadOnlyList<IOnlineClient> GetAllClients();

        /// <summary>
        /// Gets all online clients by user id.
        /// </summary>
        /// <param name="user">user identifier</param>
        IReadOnlyList<IOnlineClient> GetAllByUserId([NotNull] IUserIdentifier user);
    }
}