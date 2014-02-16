namespace Abp.Application.Authorization
{
    /// <summary>
    /// This class is used as "null object" for <see cref="IAuthorizationService"/>.
    /// It returns true for permission checks.
    /// </summary>
    public sealed class NullAuthorizationService : IAuthorizationService
    {
        public bool HasAnyOfPermissions(string[] permissions)
        {
            return true;
        }

        public bool HasAllOfPermissions(string[] permissions)
        {
            return true;
        }

        /// <summary>
        /// Gets Singleton instance of <see cref="NullAuthorizationService"/>.
        /// </summary>
        public static NullAuthorizationService Instance { get { return SingletonInstance; } }
        private static readonly NullAuthorizationService SingletonInstance = new NullAuthorizationService();

        /// <summary>
        /// Private constructor to disable instancing.
        /// </summary>
        private NullAuthorizationService()
        {
            
        }
    }
}