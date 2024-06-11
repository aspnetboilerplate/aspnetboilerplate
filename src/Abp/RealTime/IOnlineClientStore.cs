using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.RealTime
{
    public interface IOnlineClientStore
    {
        /// <summary>
        /// Adds a client.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns>The task to handle async operation</returns>
        Task AddAsync(IOnlineClient client);

        /// <summary>
        /// Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <returns>true if the client is removed, otherwise, false</returns>
        Task<bool> RemoveAsync(string connectionId);

        /// <summary>
        /// Removes a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <param name="clientAction">The Action for setup value for client.</param>
        /// <returns>true if the client is removed, otherwise, false</returns>
        Task<bool> TryRemoveAsync(string connectionId, Action<IOnlineClient> clientAction);

        /// <summary>
        /// Gets a client by connection id.
        /// </summary>
        /// <param name="connectionId">The connection id.</param>
        /// <param name="clientAction">The Action for setup value for client.</param>
        /// <returns>true if the client exists, otherwise, false</returns>
        Task<bool> TryGetAsync(string connectionId, Action<IOnlineClient> clientAction);

        /// <summary>
        /// Gets all online clients.
        /// </summary>
        Task<IReadOnlyList<IOnlineClient>> GetAllAsync();


        /// <summary>
        /// Gets all online clients by user identifier.
        /// </summary>
        /// <param name="userIdentifier">user identifier with tenant id and user id</param>
        Task<IReadOnlyList<IOnlineClient>> GetAllByUserIdAsync(UserIdentifier userIdentifier);
    }
}
