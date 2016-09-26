using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Abp.RealTime
{
    /// <summary>
    /// Extension methods for <see cref="IOnlineClientManager"/>.
    /// </summary>
    public static class OnlineClientManagerExtensions
    {
        /// <summary>
        /// Determines whether the specified user is online or not.
        /// </summary>
        /// <param name="onlineClientManager">The online client manager.</param>
        /// <param name="user">User.</param>
        public static bool IsOnline(this IOnlineClientManager onlineClientManager, UserIdentifier user)
        {
            return onlineClientManager.GetAllByUserId(user).Any();
        }

        public static IReadOnlyList<IOnlineClient> GetAllByUserId(this IOnlineClientManager onlineClientManager, IUserIdentifier user)
        {
            return onlineClientManager.GetAllClients()
                 .Where(c => c.UserId == user.UserId && c.TenantId == user.TenantId)
                 .ToImmutableList();
        }

        public static bool Remove(this IOnlineClientManager onlineClientManager, IOnlineClient client)
        {
            return onlineClientManager.Remove(client.ConnectionId);
        }
    }
}