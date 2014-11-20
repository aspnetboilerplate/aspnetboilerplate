namespace Abp.Authorization
{
    /// <summary>
    /// Null (and default) implementation of <see cref="IPermissionGrantStore"/>.
    /// </summary>
    public sealed class NullPermissionGrantStore : IPermissionGrantStore
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullPermissionGrantStore Instance { get { return SingletonInstance; } }
        private static readonly NullPermissionGrantStore SingletonInstance = new NullPermissionGrantStore();

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

        private NullPermissionGrantStore()
        {
            
        }
    }
}