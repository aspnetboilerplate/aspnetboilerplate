using System.Collections.Generic;
using Abp.Data;

namespace Abp.RealTime
{
    public interface IOnlineClientStore<T> : IOnlineClientStore
    {

    }

    public interface IOnlineClientStore
    {
        /// <summary>
        /// Adds a client.
        /// </summary>
        /// <param name="client">The client.</param>
        void Add(IOnlineClient client);

        /// <summary>
        /// Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>true if the client is removed, otherwise, false</returns>
        bool Remove(string connectionId);

        /// <summary>
        /// Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>true if the client is removed, otherwise, false</returns>
        bool TryRemove(string connectionId, out IOnlineClient client);

        /// <summary>
        /// Gets a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>true if the client exists, otherwise, false</returns>
        bool TryGet(string connectionId, out IOnlineClient client);

        /// <summary>
        /// Determines if store contains client with connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        bool Contains(string connectionId);

        /// <summary>
        /// Gets all online clients.
        /// </summary>
        IReadOnlyList<IOnlineClient> GetAll();
    }
}
