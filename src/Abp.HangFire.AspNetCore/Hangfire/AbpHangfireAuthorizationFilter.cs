using Abp.Authorization;
using Abp.Extensions;
using Abp.Runtime.Session;
using Hangfire.Dashboard;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.Hangfire
{
    public class AbpHangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly string _requiredPermissionName;

        public AbpHangfireAuthorizationFilter(string requiredPermissionName = null)
        {
            _requiredPermissionName = requiredPermissionName;
        }

        public bool Authorize(DashboardContext context)
        {
            if (!IsLoggedIn(context))
            {
                return false;
            }

            if (!_requiredPermissionName.IsNullOrEmpty() && !IsPermissionGranted(context, _requiredPermissionName))
            {
                return false;
            }

            return true;
        }

        private static bool IsLoggedIn(DashboardContext context)
        {
            var abpSession = context.GetHttpContext().RequestServices.GetRequiredService<IAbpSession>();
            return abpSession.UserId.HasValue;
        }

        private static bool IsPermissionGranted(DashboardContext context, string requiredPermissionName)
        {
            var permissionChecker = context.GetHttpContext().RequestServices.GetRequiredService<IPermissionChecker>();
            return permissionChecker.IsGranted(requiredPermissionName);
        }
    }
}
