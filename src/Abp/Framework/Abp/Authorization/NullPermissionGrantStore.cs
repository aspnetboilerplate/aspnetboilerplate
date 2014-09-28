namespace Abp.Authorization
{
    /// <summary>
    /// Null (and default) implementation of <see cref="IPermissionGrantStore"/>.
    /// </summary>
    public class NullPermissionGrantStore : IPermissionGrantStore
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullPermissionGrantStore Instance { get { return SingletonInstance; } }
        private static readonly NullPermissionGrantStore SingletonInstance = new NullPermissionGrantStore();

        public bool IsGranted(long userId, string permissionName)
        {
            return true;
        }

        private NullPermissionGrantStore()
        {
            
        }
    }
}