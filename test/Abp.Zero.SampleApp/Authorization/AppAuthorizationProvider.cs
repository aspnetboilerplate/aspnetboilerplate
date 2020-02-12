using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;
using Abp.MultiTenancy;
using Abp.Zero.SampleApp.Features;

namespace Abp.Zero.SampleApp.Authorization
{
    public class AppAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var permission1 = context.CreatePermission("Permission1", new FixedLocalizableString("Permission1"));
            context.CreatePermission("Permission2", new FixedLocalizableString("Permission2"));
            context.CreatePermission("Permission3", new FixedLocalizableString("Permission3"));
            context.CreatePermission("Permission4", new FixedLocalizableString("Permission4"));
            context.CreatePermission("Permission5", new FixedLocalizableString("Permission5"), multiTenancySides: MultiTenancySides.Host);
            context.CreatePermission("PermissionWithFeatureDependency", new FixedLocalizableString("PermissionWithFeatureDependency"), featureDependency: new SimpleFeatureDependency(AppFeatureProvider.MyBoolFeature));

            var firstLevelChilPermission1 = permission1.CreateChildPermission("FirstLevelChilPermission1", new FixedLocalizableString("FirstLevelChilPermission1"));
            firstLevelChilPermission1.CreateChildPermission("SecondLevelChildPermission1", new FixedLocalizableString("SecondLevelChildPermission1"));
        }
    }
}
