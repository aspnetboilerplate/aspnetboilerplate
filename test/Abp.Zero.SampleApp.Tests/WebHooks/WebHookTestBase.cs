using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookTestBase : SampleAppTestBase<WebHookPublishTestModule>
    {
        protected T RegisterFake<T>() where T : class
        {
            var substitute = Substitute.For<T>();
            LocalIocManager.IocContainer.Register(Component.For<T>().Instance(substitute).IsDefault());
            return substitute;
        }
    }

    public class TestWebHookDefinitionProvider : WebHookDefinitionProvider
    {
        public override void SetWebHooks(IWebHookDefinitionContext context)
        {
            context.Manager.Add(
                new WebHookDefinition(
                    "Test",
                    L("TestWebHook"),
                    L("DefaultDescription")
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    "user_created",
                    L("UserCreatedWebHook"),
                    L("DescriptionCreated"),
                    new SimplePermissionDependency(AppPermissions.WebHookPermission)
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    "admin.tenant_deleted",
                    L("TenantDeletedWebHook"),
                    L("DescriptionTenantDeleted"),
                    new SimplePermissionDependency(true, AppPermissions.WebHookPermission, AppPermissions.WebHook.TenantDeleted)
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    "chat.new_message",
                    L("NewMessageWebHook"),
                    L("TriggersWhenYouGetANewMessage"),
                    new SimplePermissionDependency(AppPermissions.WebHookPermission),
                    new SimpleFeatureDependency(AppFeatures.ChatFeature)
                ));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroConsts.LocalizationSourceName);
        }
    }

    public class WebHookPublishTestModule : SampleAppTestModule
    {
        public override void PreInitialize()
        {
            Configuration.WebHooks.Providers.Add<TestWebHookDefinitionProvider>();

            IocManager.RegisterAssemblyByConvention(typeof(WebHookPublishTestModule).Assembly);
            base.PreInitialize();
        }
    }
}
