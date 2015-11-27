using Abp.Authorization;
using Abp.Localization;

namespace Abp.Zero.SampleApp.Authorization
{
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission("Permission1", new FixedLocalizableString("Permission1"));
            context.CreatePermission("Permission2", new FixedLocalizableString("Permission2"));
            context.CreatePermission("Permission3", new FixedLocalizableString("Permission3"), true);
            context.CreatePermission("Permission4", new FixedLocalizableString("Permission4"), true);
        }
    }
}
