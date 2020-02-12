using Abp.Authorization;
using Abp.Localization;

namespace Abp.Web.Common.Tests
{
    public class AbpWebCommonTestModuleAuthProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission("Permission1", new FixedLocalizableString("Permission1"));
        }
    }
}