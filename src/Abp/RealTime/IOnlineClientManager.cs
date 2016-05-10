using System.Collections.Generic;

namespace Abp.RealTime
{
    /// <summary>
    ///     Used to manage online clients those are connected to the application..
    /// </summary>
    public interface IOnlineClientManager
    {
        /// <summary>
        ///     Adds a client.
        /// </summary>
        /// <param name="client">The client.</param>
        void Add(IOnlineClient client);

        /// <summary>
        ///     Removes the specified client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>True, if a client is removed</returns>
        bool Remove(IOnlineClient client);

        /// <summary>
        ///     Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>True, if a client is removed</returns>
        bool Remove(string connectionId);

        /// <summary>
        ///     Tries to find a client by connection id.
        ///     Returns null if not found.
        /// </summary>
        /// <param name="connectionId">connection id</param>
        IOnlineClient GetByConnectionIdOrNull(string connectionId);

        /// <summary>
        ///     Tries to find a client by userId.
        ///     Returns null if not found.
        /// </summary>
        /// <param name="user">User.</param>
        IOnlineClient GetByUserIdOrNull(IUserIdentifier user);

        /// <summary>
        ///     Gets all online clients.
        /// </summary>
        IReadOnlyList<IOnlineClient> GetAllClients();
    }
}