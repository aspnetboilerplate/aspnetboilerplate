using Abp.Authorization;
using Abp.MultiTenancy;

namespace Abp.ZeroCore.SampleApp.Application
{
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(AppPermissions.TestPermission, AppLocalizationHelper.L("TestPermission"), multiTenancySides: MultiTenancySides.Tenant);
        }
    }
}
