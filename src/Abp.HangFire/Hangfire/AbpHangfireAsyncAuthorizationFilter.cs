using Abp.Authorization;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Runtime.Session;
using Hangfire.Dashboard;
using System.Threading.Tasks;

namespace Abp.Hangfire
{
    /// <summary>
    /// Hangfire authorization filter that uses ABP's permission system to authorize users.
    /// </summary>
    public class AbpHangfireAsyncAuthorizationFilter : IDashboardAsyncAuthorizationFilter
    {
        private readonly IIocResolver _iocResolver;

        private readonly string _requiredPermissionName;

        public AbpHangfireAsyncAuthorizationFilter(string requiredPermissionName = null)
        {
            _requiredPermissionName = requiredPermissionName;

            _iocResolver = IocManager.Instance;
        }

        /// <summary>
        /// Asynchronously authorize the current user to access the Hangfire dashboard.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<bool> AuthorizeAsync(DashboardContext context)
        {
            return IsLoggedIn() && (_requiredPermissionName.IsNullOrEmpty() || await IsPermissionGrantedAsync(_requiredPermissionName));
        }

        /// <summary>
        /// Check if the user is logged in by checking if the user id is set in the session.
        /// </summary>
        /// <returns></returns>
        private bool IsLoggedIn()
        {
            using (var abpSession = _iocResolver.ResolveAsDisposable<IAbpSession>())
            {
                return abpSession.Object.UserId.HasValue;
            }
        }

        /// <summary>
        /// Check if the current user has the required permission.
        /// </summary>
        /// <param name="requiredPermissionName"></param>
        /// <returns></returns>
        private async Task<bool> IsPermissionGrantedAsync(string requiredPermissionName)
        {
            using (var permissionChecker = _iocResolver.ResolveAsDisposable<IPermissionChecker>())
            {
                return await permissionChecker.Object.IsGrantedAsync(requiredPermissionName);
            }
        }
    }
}
