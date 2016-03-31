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
        /// <param name="userId">User id.</param>
        public static bool IsOnline(IOnlineClientManager onlineClientManager,long userId)
        {
            return onlineClientManager.GetByUserIdOrNull(userId) != null;
        }
    }
}