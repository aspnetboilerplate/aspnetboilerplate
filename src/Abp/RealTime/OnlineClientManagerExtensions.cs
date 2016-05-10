namespace Abp.RealTime
{
    /// <summary>
    ///     Extension methods for <see cref="IOnlineClientManager" />.
    /// </summary>
    public static class OnlineClientManagerExtensions
    {
        /// <summary>
        ///     Determines whether the specified user is online or not.
        /// </summary>
        /// <param name="onlineClientManager">The online client manager.</param>
        /// <param name="user">User.</param>
        public static bool IsOnline(IOnlineClientManager onlineClientManager, UserIdentifier user)
        {
            return onlineClientManager.GetByUserIdOrNull(user) != null;
        }
    }
}