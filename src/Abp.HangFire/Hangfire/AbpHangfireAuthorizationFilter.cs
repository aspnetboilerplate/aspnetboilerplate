using Abp.Authorization;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Runtime.Session;
using Hangfire.Dashboard;

namespace Abp.Hangfire
{
    public class AbpHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public IIocResolver IocResolver { get; set; }

        private readonly string _requiredPermissionName;

        public AbpHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            _requiredPermissionName = requiredPermissionName;

            IocResolver = IocManager.Instance;
        }

        public bool Authorize(DashboardContext context)
        {
            if (!IsLoggedIn())
            {
                return false;
            }

            if (!_requiredPermissionName.IsNullOrEmpty() && !IsPermissionGranted(_requiredPermissionName))
            {
                return false;
            }

            return true;
        }

        private bool IsLoggedIn()
        {
            using (var abpSession = IocResolver.ResolveAsDisposable<IAbpSession>())
            {
                return abpSession.Object.UserId.HasValue;
            }
        }

        private bool IsPermissionGranted(string requiredPermissionName)
        {
            using (var permissionChecker = IocResolver.ResolveAsDisposable<IPermissionChecker>())
            {
                return permissionChecker.Object.IsGranted(requiredPermissionName);
            }
        }
    }
}
