namespace Abp.Application.Authorization
{
    /// <summary>
    /// 
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

        public bool HasPermission(string permissionName)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public static NullAuthorizationService Instance { get { return _instance; } }

        private static NullAuthorizationService _instance;

        static NullAuthorizationService()
        {
            _instance = new NullAuthorizationService();
        }
        private NullAuthorizationService()
        {
            
        }
    }
}