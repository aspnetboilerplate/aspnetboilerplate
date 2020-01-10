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
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using Abp.Zero.SampleApp.MultiTenancy;
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
        /// <param name="tenantFeatures">Feature what will be added to created tenant</param>
        /// <returns>User and subscription</returns>

        protected async Task<WebHookSubscription> CreateTenantAndSubscribeToWebhookAsync(List<string> webHookDefinitionNames, Dictionary<string, string> tenantFeatures = null)
        {
            var tenantId = await CreateAndGetTenantIdWithFeaturesAsync(tenantFeatures);

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();

            var subscription = new WebHookSubscription
            {
                TenantId = tenantId,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = webHookDefinitionNames,
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            return subscription;
        }

        /// <summary>
        /// Creates tenant, adds given feature with given value, then subscribes to webhook
        /// </summary>
        /// <param name="webHookDefinitionName">webhook name to subscribe</param>
        /// <param name="tenantFeatureKey"></param>
        /// <param name="tenantFeatureValue"></param>
        /// <returns></returns>
        protected Task<WebHookSubscription> CreateTenantAndSubscribeToWebhookAsync(string webHookDefinitionName, string tenantFeatureKey = null, string tenantFeatureValue = null)
        {
            return CreateTenantAndSubscribeToWebhookAsync(new List<string> { webHookDefinitionName },
                new Dictionary<string, string> { { tenantFeatureKey, tenantFeatureValue } });
        }

        /// <summary>
        /// Creates tenant, adds given feature with given value, then subscribes to webhook
        /// </summary>
        /// <param name="webHookDefinitionName">webhook name to subscribe</param>
        /// <param name="tenantFeatureKey"></param>
        /// <param name="tenantFeatureValue"></param>
        /// <returns></returns>
        protected Task<WebHookSubscription> CreateTenantAndSubscribeToWebhookAsync(string webHookDefinitionName, Dictionary<string, string> tenantFeatures = null)
        {
            return CreateTenantAndSubscribeToWebhookAsync(new List<string> { webHookDefinitionName }, tenantFeatures);
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
                    new SimpleFeatureDependency(AppFeatures.WebHookFeature)
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    AppWebHookDefinitionNames.Users.Deleted,
                    L("DeletedDeletedWebHook"),
                    L("DescriptionDeletedDeleted"),
                    new SimpleFeatureDependency(false, AppFeatures.WebHookFeature, AppFeatures.TestFeature)
                ));

            context.Manager.Add(
                new WebHookDefinition(
                    AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                    L("DefaultThemeChanged"),
                    L("TriggersWhenDefaultThemeChanged"),
                    new SimpleFeatureDependency(true, AppFeatures.WebHookFeature, AppFeatures.ThemeFeature)
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
               AppFeatures.WebHookFeature,
               defaultValue: "false",
               displayName: L("WebHookFeature"),
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

            Configuration.Features.Providers.Add<TestWebHookFeatureProvider>();

            IocManager.RegisterAssemblyByConvention(typeof(WebHookPublishTestModule).Assembly);
        }
    }
}
