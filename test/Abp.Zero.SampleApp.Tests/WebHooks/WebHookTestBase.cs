using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Localization;
using Abp.Modules;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using Abp.Zero.SampleApp.Users;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookTestBase : SampleAppTestBase<WebHookPublishTestModule>
    {
        protected IWebHookSubscriptionManager WebHookSubscriptionManager;
        private UserManager _userManager;

        public WebHookTestBase()
        {
            WebHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            _userManager = Resolve<UserManager>();
        }

        protected T RegisterFake<T>() where T : class
        {
            var substitute = Substitute.For<T>();
            LocalIocManager.IocContainer.Register(Component.For<T>().Instance(substitute).IsDefault());
            return substitute;
        }

        /// <summary>
        /// Creates user, gives him given permissions than subscribe to webhooks.
        /// </summary>
        /// <param name="webHookDefinitionNames">WebHook to subscribe</param>
        /// <param name="permissions">User's permission</param>
        /// <returns>User and subscription</returns>

        protected async Task<(User user, WebHookSubscription webHookSubscription)> CreateUserAndSubscribeToWebhook(List<string> webHookDefinitionNames, List<string> permissions = null)
        {
            var user = await CreateNewUserWithWebhookPermissions(permissions);
            var subscription = new WebHookSubscription()
            {
                TenantId = AbpSession.TenantId,
                UserId = user.Id,
                Secret = "secret",
                WebHookUri = "www.mywehhook.com",
                WebHookDefinitions = webHookDefinitionNames,
                Headers = new Dictionary<string, string>()
                {
                    { "Key","Value"}
                }
            };

            await WebHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            return (user, subscription);
        }

        protected Task<(User user, WebHookSubscription webHookSubscription)> CreateUserAndSubscribeToWebhook(string webHookDefinitionNames, string permissions = null)
        {
            return CreateUserAndSubscribeToWebhook(new List<string>() { webHookDefinitionNames },
                new List<string>() { permissions });
        }

        protected async Task<User> CreateNewUserWithWebhookPermissions(List<string> permissions = null)
        {
            var user = await CreateUser(Guid.NewGuid().ToString().Replace("-", ""));

            await GrantPermissionAsync(user, AppPermissions.WebHookMainPermission);

            if (permissions != null && permissions.Any(p => !string.IsNullOrWhiteSpace(p)))
            {
                permissions = permissions.Where(p => !string.IsNullOrWhiteSpace(p)).ToList();

                foreach (var permission in permissions)
                {
                    await GrantPermissionAsync(user, permission);
                }
            }

            return user;
        }
    }

    public class TestWebHookAuthorizationProvider : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Create a root permission group for 'Administration' permissions
            var administration = context.CreatePermission("Abp.Zero.Administration", new FixedLocalizableString("Administration"));

            //Create 'User management' permission under 'Administration' group
            var userManagement = administration.CreateChildPermission("Abp.Zero.Administration.UserManagement", new FixedLocalizableString("User management"));

            //Create 'Change permissions' (to be able to change permissions of a user) permission as child of 'User management' permission.
            userManagement.CreateChildPermission("Abp.Zero.Administration.UserManagement.ChangePermissions", new FixedLocalizableString("Change permissions"));

            var mainPermission = context.CreatePermission(AppPermissions.WebHookMainPermission, new FixedLocalizableString(AppPermissions.WebHookMainPermission));

            var tenantPermission = mainPermission.CreateChildPermission(AppPermissions.WebHook.TenantMainPermission,
                new FixedLocalizableString(AppPermissions.WebHook.TenantMainPermission));

            tenantPermission.CreateChildPermission(AppPermissions.WebHook.Tenant.TenantDeleted,
               new FixedLocalizableString(AppPermissions.WebHook.Tenant.TenantDeleted));

            mainPermission.CreateChildPermission(AppPermissions.WebHook.UserCreated, new FixedLocalizableString(AppPermissions.WebHook.UserCreated));
        }
    }


    public class TestWebHookDefinitionProvider : WebHookDefinitionProvider
    {
        public override void SetWebHooks(IWebHookDefinitionContext context)
        {
            context.Manager.Add(
                new WebHookDefinition(
                    AppWebHookDefinitionNames.Test,
                    L("TestWebHook"),
                    L("DefaultDescription")
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    AppWebHookDefinitionNames.Users.Created,
                    L("UserCreatedWebHook"),
                    L("DescriptionCreated"),
                    new SimplePermissionDependency(AppPermissions.WebHook.UserCreated)
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    AppWebHookDefinitionNames.Tenant.Deleted,
                    L("TenantDeletedWebHook"),
                    L("DescriptionTenantDeleted"),
                    new SimplePermissionDependency(true, AppPermissions.WebHookMainPermission, AppPermissions.WebHook.Tenant.TenantDeleted)
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    AppWebHookDefinitionNames.Chat.NewMessageReceived,
                    L("NewMessageWebHook"),
                    L("TriggersWhenYouGetANewMessage"),
                    new SimplePermissionDependency(AppPermissions.WebHookMainPermission),
                    new SimpleFeatureDependency(AppFeatures.ChatFeature)
                ));
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroConsts.LocalizationSourceName);
        }
    }

    [DependsOn(typeof(SampleAppTestModule))]
    public class WebHookPublishTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.WebHooks.Providers.Add<TestWebHookDefinitionProvider>();

            Configuration.Authorization.Providers.Add<TestWebHookAuthorizationProvider>();

            IocManager.RegisterAssemblyByConvention(typeof(WebHookPublishTestModule).Assembly);
        }
    }
}
