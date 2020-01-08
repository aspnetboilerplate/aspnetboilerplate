using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.UI.Inputs;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using Abp.Zero.SampleApp.Users;
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
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = webHookDefinitionNames,
                Headers = new Dictionary<string, string>()
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

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

        public async Task AddOrReplaceFeatureToTenant(int tenantId, string featureName, string featureValue)
        {
            await WithUnitOfWorkAsync(async () =>
            {
                var tenantFeatureRepository = Resolve<IRepository<TenantFeatureSetting, long>>();
                if (await tenantFeatureRepository.GetAll().AnyAsync(x => x.TenantId == tenantId && x.Name == featureName))
                {
                    await tenantFeatureRepository.DeleteAsync(x => x.TenantId == tenantId && x.Name == featureName);
                }

                await tenantFeatureRepository.InsertAsync(new TenantFeatureSetting(tenantId, featureName, featureValue));
            });
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
                    new SimplePermissionDependency(true, AppPermissions.WebHookMainPermission, AppPermissions.WebHook.TenantMainPermission, AppPermissions.WebHook.Tenant.TenantDeleted)
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

        private ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroConsts.LocalizationSourceName);
        }
    }

    public class TestWebHookFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            var chatFeature = context.Create(
               AppFeatures.ChatFeature,
               defaultValue: "false",
               displayName: L("ChatFeature"),
               inputType: new CheckboxInputType()
           );
        }

        private ILocalizableString L(string name)
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

            Configuration.Features.Providers.Add<TestWebHookFeatureProvider>();

            IocManager.RegisterAssemblyByConvention(typeof(WebHookPublishTestModule).Assembly);
        }
    }
}
