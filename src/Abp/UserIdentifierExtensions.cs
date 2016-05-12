namespace Abp
{
    /// <summary>
    /// Extension methods for <see cref="UserIdentifier"/> and <see cref="IUserIdentifier"/>.
    /// </summary>
    public static class UserIdentifierExtensions
    {
        /// <summary>
        /// Creates a new <see cref="UserIdentifier"/> object from any object implements <see cref="IUserIdentifier"/>.
        /// </summary>
        /// <param name="userIdentifier">User identifier.</param>
        public static UserIdentifier ToUserIdentifier(this IUserIdentifier userIdentifier)
        {
            return new UserIdentifier(userIdentifier.TenantId, userIdentifier.UserId);
        }
    }
}