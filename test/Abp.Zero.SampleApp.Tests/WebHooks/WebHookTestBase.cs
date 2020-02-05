using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Localization;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.UI.Inputs;
using Abp.Webhooks;
using Abp.Zero.SampleApp.Application;
using Abp.Zero.SampleApp.MultiTenancy;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class WebhookTestBase : SampleAppTestBase<WebhookPublishTestModule>
    {
        protected const string WebhookSubscriptionSecretPrefix = "whs_";

        protected T RegisterFake<T>() where T : class
        {
            var substitute = Substitute.For<T>();
            LocalIocManager.IocContainer.Register(Component.For<T>().Instance(substitute).IsDefault());
            return substitute;
        }

        /// <summary>
        /// Creates user, gives him given permissions than subscribe to webhooks.
        /// </summary>
        /// <param name="webhookNames">Webhook to subscribe</param>
        /// <param name="tenantFeatures">Feature what will be added to created tenant</param>
        /// <returns>User and subscription</returns>

        protected async Task<WebhookSubscription> CreateTenantAndSubscribeToWebhookAsync(List<string> webhookNames, Dictionary<string, string> tenantFeatures = null)
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(tenantFeatures);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var subscription = new WebhookSubscription
            {
                TenantId = tenantId,
                WebhookUri = "www.mywebhook.com",
                Webhooks = webhookNames,
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            return subscription;
        }

        /// <summary>
        /// Creates tenant, adds given feature with given value, then subscribes to webhook
        /// </summary>
        /// <param name="webhookDefinitionName">webhook name to subscribe</param>
        /// <param name="tenantFeatureKey"></param>
        /// <param name="tenantFeatureValue"></param>
        /// <returns></returns>
        protected Task<WebhookSubscription> CreateTenantAndSubscribeToWebhookAsync(string webhookDefinitionName, string tenantFeatureKey = null, string tenantFeatureValue = null)
        {
            return CreateTenantAndSubscribeToWebhookAsync(new List<string> { webhookDefinitionName },
                new Dictionary<string, string> { { tenantFeatureKey, tenantFeatureValue } });
        }

        /// <summary>
        /// Creates tenant, adds given feature with given value, then subscribes to webhook
        /// </summary>
        /// <param name="webhookDefinitionName">webhook name to subscribe</param>
        /// <param name="tenantFeatures"></param>
        /// <returns></returns>
        protected Task<WebhookSubscription> CreateTenantAndSubscribeToWebhookAsync(string webhookDefinitionName, Dictionary<string, string> tenantFeatures = null)
        {
            return CreateTenantAndSubscribeToWebhookAsync(new List<string> { webhookDefinitionName }, tenantFeatures);
        }

        protected async Task<int> CreateAndGetTenantIdWithFeaturesAsync(string featureKey, string featureValue)
        {
            return await CreateAndGetTenantIdWithFeaturesAsync(
                new Dictionary<string, string>()
                {
                    {featureKey, featureValue}
                });
        }
        /// <summary>
        /// Creates tenant with given features. Returns created tenant's id
        /// </summary>
        /// <param name="tenantFeatures"></param>
        protected async Task<int> CreateAndGetTenantIdWithFeaturesAsync(Dictionary<string, string> tenantFeatures = null)
        {
            string name = Guid.NewGuid().ToString().Replace("-", "");

            var tenant = new Tenant(name, name);
            var tenantId = await Resolve<IRepository<Tenant>>().InsertAndGetIdAsync(tenant);

            if (tenantFeatures != null)
            {
                foreach (var tenantFeature in tenantFeatures.Where(f => !string.IsNullOrWhiteSpace(f.Key)))
                {
                    await AddOrReplaceFeatureToTenantAsync(tenantId, tenantFeature.Key, tenantFeature.Value);
                }
            }

            return tenantId;
        }

        protected async Task AddOrReplaceFeatureToTenantAsync(int tenantId, string featureName, string featureValue)
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

    public class TestWebhookDefinitionProvider : WebhookDefinitionProvider
    {
        public override void SetWebhooks(IWebhookDefinitionContext context)
        {
            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Test,
                    L("TestWebhook"),
                    L("DefaultDescription")
                ));

            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Users.Created,
                    L("UserCreatedWebhook"),
                    L("DescriptionCreated"),
                    new SimpleFeatureDependency(AppFeatures.WebhookFeature)
                ));

            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Users.Deleted,
                    L("DeletedDeletedWebhook"),
                    L("DescriptionDeletedDeleted"),
                    new SimpleFeatureDependency(false, AppFeatures.WebhookFeature, AppFeatures.TestFeature)
                ));

            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                    L("DefaultThemeChanged"),
                    L("TriggersWhenDefaultThemeChanged"),
                    new SimpleFeatureDependency(true, AppFeatures.WebhookFeature, AppFeatures.ThemeFeature)
                ));
        }

        private ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroConsts.LocalizationSourceName);
        }
    }

    public class TestWebhookFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                 AppFeatures.TestFeature,
                 defaultValue: "false",
                 displayName: L("TestFeature"),
                 inputType: new CheckboxInputType()
             );

            context.Create(
                AppFeatures.ThemeFeature,
                defaultValue: "false",
                displayName: L("ChatFeature"),
                inputType: new CheckboxInputType()
            );

            context.Create(
               AppFeatures.WebhookFeature,
               defaultValue: "false",
               displayName: L("WebhookFeature"),
               inputType: new CheckboxInputType()
           );
        }

        private ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroConsts.LocalizationSourceName);
        }
    }

    [DependsOn(typeof(SampleAppTestModule))]
    public class WebhookPublishTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Webhooks.Providers.Add<TestWebhookDefinitionProvider>();

            Configuration.Features.Providers.Add<TestWebhookFeatureProvider>();

            IocManager.RegisterAssemblyByConvention(typeof(WebhookPublishTestModule).Assembly);
        }
    }
}
