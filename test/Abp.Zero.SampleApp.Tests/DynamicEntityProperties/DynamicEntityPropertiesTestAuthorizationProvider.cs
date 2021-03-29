using Abp.Authorization;
using Abp.Localization;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class DynamicEntityPropertiesTestAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission(DynamicEntityPropertiesTestBase.TestPermission,
                new FixedLocalizableString(DynamicEntityPropertiesTestBase.TestPermission));
        }
    }
}