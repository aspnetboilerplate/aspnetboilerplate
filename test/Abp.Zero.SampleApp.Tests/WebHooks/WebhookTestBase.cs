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
        protected WebhookSubscription CreateTenantAndSubscribeToWebhook(List<string> webhookNames,
            Dictionary<string, string> tenantFeatures = null)
        {
            var tenantId = CreateAndGetTenantIdWithFeatures(tenantFeatures);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            var subscription = new WebhookSubscription
            {
                TenantId = tenantId,
                WebhookUri = "www.mywebhook.com",
                Webhooks = webhookNames,
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value"}
                }
            };

            webhookSubscriptionManager.AddOrUpdateSubscription(subscription);

            return subscription;
        }

        /// <summary>
        /// Creates tenant, adds given feature with given value, then subscribes to webhook
        /// </summary>
        /// <param name="webhookDefinitionName">webhook name to subscribe</param>
        /// <param name="tenantFeatureKey"></param>
        /// <param name="tenantFeatureValue"></param>
        /// <returns></returns>
        protected WebhookSubscription CreateTenantAndSubscribeToWebhook(string webhookDefinitionName,
            string tenantFeatureKey = null, string tenantFeatureValue = null)
        {
            return CreateTenantAndSubscribeToWebhook(
                new List<string> {webhookDefinitionName},
                new Dictionary<string, string>
                {
                    {
                        tenantFeatureKey, tenantFeatureValue
                    }
                }
            );
        }

        /// <summary>
        /// Creates tenant, adds given feature with given value, then subscribes to webhook
        /// </summary>
        /// <param name="webhookDefinitionName">webhook name to subscribe</param>
        /// <param name="tenantFeatures"></param>
        /// <returns></returns>
        protected WebhookSubscription CreateTenantAndSubscribeToWebhook(string webhookDefinitionName,
            Dictionary<string, string> tenantFeatures = null)
        {
            return CreateTenantAndSubscribeToWebhook(new List<string>
                {
                    webhookDefinitionName
                }, tenantFeatures
            );
        }

        protected int CreateAndGetTenantIdWithFeatures(string featureKey, string featureValue)
        {
            return CreateAndGetTenantIdWithFeatures(
                new Dictionary<string, string>()
                {
                    {featureKey, featureValue}
                });
        }

        /// <summary>
        /// Creates tenant with given features. Returns created tenant's id
        /// </summary>
        /// <param name="tenantFeatures"></param>
        protected int CreateAndGetTenantIdWithFeatures(Dictionary<string, string> tenantFeatures = null)
        {
            var name = Guid.NewGuid().ToString().Replace("-", "");

            var tenant = new Tenant(name, name);
            var tenantId = Resolve<IRepository<Tenant>>().InsertAndGetId(tenant);

            if (tenantFeatures != null)
            {
                foreach (var tenantFeature in tenantFeatures.Where(f => !string.IsNullOrWhiteSpace(f.Key)))
                {
                    AddOrReplaceFeatureToTenant(tenantId, tenantFeature.Key, tenantFeature.Value);
                }
            }

            return tenantId;
        }

        protected void AddOrReplaceFeatureToTenant(int tenantId, string featureName, string featureValue)
        {
            WithUnitOfWork(() =>
            {
                var tenantFeatureRepository = Resolve<IRepository<TenantFeatureSetting, long>>();
                if (tenantFeatureRepository.GetAll().Any(x => x.TenantId == tenantId && x.Name == featureName))
                {
                    tenantFeatureRepository.Delete(x => x.TenantId == tenantId && x.Name == featureName);
                }

                tenantFeatureRepository.Insert(new TenantFeatureSetting(tenantId, featureName, featureValue));
            });
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
