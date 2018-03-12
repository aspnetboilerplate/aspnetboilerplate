using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;
using Abp.Zero.SampleApp.Features;

namespace Abp.Zero.SampleApp.Authorization
{
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            context.CreatePermission("Permission1", new FixedLocalizableString("Permission1"));
            context.CreatePermission("Permission2", new FixedLocalizableString("Permission2"));
            context.CreatePermission("Permission3", new FixedLocalizableString("Permission3"));
            context.CreatePermission("Permission4", new FixedLocalizableString("Permission4"));
            context.CreatePermission("PermissionWithFeatureDependency", new FixedLocalizableString("PermissionWithFeatureDependency"), featureDependency:new SimpleFeatureDependency(AppFeatureProvider.MyBoolFeature));
        }
    }
}
