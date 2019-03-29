using System.Collections.Generic;
using Abp.Data;

namespace Abp.RealTime
{
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
        /// <returns>Tuple indicating whether the client was removed and if so, the value</returns>
        ConditionalValue<IOnlineClient> Remove(string connectionId);

        /// <summary>
        /// Get's a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        ConditionalValue<IOnlineClient> Get(string connectionId);

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
