namespace Abp.Authorization
{
    /// <summary>
    /// Null (and default) implementation of <see cref="IPermissionChecker"/>.
    /// </summary>
    public sealed class NullPermissionChecker : IPermissionChecker
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullPermissionChecker Instance { get { return SingletonInstance; } }
        private static readonly NullPermissionChecker SingletonInstance = new NullPermissionChecker();

        public bool IsGranted(string permissionName)
        {
            return true;
        }

        /// <summary>
        /// Checks if a user is granted for a permission.
        /// </summary>
        /// <param name="userId">Id of the user to check</param>
        /// <param name="permissionName">Name of the permission</param>
        /// <returns><c>true</c> if this instance is granted the specified userId permissionName; otherwise, <c>false</c>.</returns>
        public bool IsGranted(long userId, string permissionName)
        {
            return true;
        }

        private NullPermissionChecker()
        {

        }
    }
}