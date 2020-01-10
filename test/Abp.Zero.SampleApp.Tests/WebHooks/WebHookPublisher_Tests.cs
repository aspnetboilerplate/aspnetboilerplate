using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Json;
using Abp.Threading;
using Abp.WebHooks;
using Abp.WebHooks.BackgroundWorker;
using Abp.Zero.SampleApp.Application;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class WebHookPublisher_Tests : WebHookTestBase
    {
        private readonly IWebHookPublisher _webhookPublisher;
        private readonly IBackgroundJobManager _backgroundJobManagerSubstitute;

        public WebHookPublisher_Tests()
        {
            AbpSession.UserId = 1;
            AbpSession.TenantId = null;

            _backgroundJobManagerSubstitute = RegisterFake<IBackgroundJobManager>();
            _webhookPublisher = Resolve<IWebHookPublisher>();
        }

        #region Async
        /// <summary>
        /// Creates tenant with adding feature(s), then creates predicate for WebHookSenderInput which publisher should send to WebHookSenderJob
        /// </summary>
        /// <param name="webhookDefinition"></param>
        /// <param name="tenantFeatures"></param>
        /// <returns></returns>
        private async Task<(int? tenantId, object data, Predicate<WebHookSenderInput> predicate)> InitializeTestCase(string webhookDefinition, Dictionary<string, string> tenantFeatures)
        {
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(webhookDefinition, tenantFeatures);

            var webHooksConfiguration = Resolve<IWebHooksConfiguration>();

            var data = new { Name = "Musa", Surname = "Demir" };

            Predicate<WebHookSenderInput> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith("whs_");
                w.WebHookDefinition.ShouldContain(webhookDefinition);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebHookSubscriptionId.ShouldBe(subscription.Id);
                w.Data.ShouldBe(
                    webHooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webHooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            return (subscription.TenantId, data, predicate);
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_If_There_Is_No_Subscription_Async()
        {
            await CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created,
               AppFeatures.WebHookFeature, "true");

            AbpSession.TenantId = GetDefaultTenant().Id;

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Test,
                new
                {
                    Name = "Musa",
                    Surname = "Demir"
                });

            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Authorized_Tenant_Async()
        {
            var (tenantId, data, predicate) = await InitializeTestCase(AppWebHookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                });

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Created, data, tenantId);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Authorized_Current_Tenant_Async()
        {
            var (tenantId, data, predicate) = await InitializeTestCase(AppWebHookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                });

            AbpSession.TenantId = tenantId;

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Created, data);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_Tenant_If_Features_Are_Not_Granted_Async()
        {
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created,
                AppFeatures.WebHookFeature, "true");

            await AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebHookFeature, "false");

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Created, new { Name = "Musa", Surname = "Demir" }, subscription.TenantId);

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_Current_Tenant_If_Features_Are_Not_Granted_Async()
        {
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created,
                AppFeatures.WebHookFeature, "true");

            AbpSession.TenantId = subscription.TenantId;

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.WebHookFeature, "false");

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Created, new { Name = "Musa", Surname = "Demir" });

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Tenant_If_All_Required_Features_Granted_Async()
        {
            //user_deleted webhook requires AppFeatures.WebHookFeature, AppFeatures.TestFeature but not requires all
            var (tenantId, data, predicate) = await InitializeTestCase(AppWebHookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                });

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Deleted, data, tenantId);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var (tenantId2, data2, predicate2) = await InitializeTestCase(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, data2, tenantId2);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate2(w)));
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Current_Tenant_If_All_Required_Features_Granted_Async()
        {
            //user_deleted webhook requires AppFeatures.WebHookFeature, AppFeatures.TestFeature but not requires all
            var (tenantId, data, predicate) = await InitializeTestCase(AppWebHookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                });

            AbpSession.TenantId = tenantId;
            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Deleted, data);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var (tenantId2, data2, predicate2) = await InitializeTestCase(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            AbpSession.TenantId = tenantId2;
            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, data2);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate2(w)));
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_If_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Async()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            //remove one feature
            await AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebHookFeature, "false");

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, new { Name = "Musa", Surname = "Demir" }, subscription.TenantId);

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_If_Current_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Async()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            //remove one feature
            await AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebHookFeature, "false");

            AbpSession.TenantId = subscription.TenantId;
            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, new { Name = "Musa", Surname = "Demir" });

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Host_If_Subscribed_Async()
        {
            var subscription = new WebHookSubscription
            {
                TenantId = null,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string>() { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var webHooksConfiguration = Resolve<IWebHooksConfiguration>();

            await webHookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            var data = new { Name = "Musa", Surname = "Demir" };

            Predicate<WebHookSenderInput> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith("whs_");
                w.WebHookDefinition.ShouldContain(AppWebHookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebHookSubscriptionId.ShouldBe(subscription.Id);
                w.Data.ShouldBe(
                    webHooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webHooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            await _webhookPublisher.PublishAsync(AppWebHookDefinitionNames.Users.Created, data, null);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        #endregion

        #region Sync

        [Fact]
        public void Should_Not_Send_Webhook_If_There_Is_Subscription_Sync()
        {
            AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created,
               AppFeatures.WebHookFeature, "true"));

            AbpSession.TenantId = GetDefaultTenant().Id;
            _webhookPublisher.Publish(AppWebHookDefinitionNames.Test,
               new
               {
                   Name = "Musa",
                   Surname = "Demir"
               });

            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Authorized_Tenant_Sync()
        {
            var (tenantId, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebHookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                }));

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Created, data, tenantId);

            _backgroundJobManagerSubstitute.Received()
               .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        [Fact]
        public void Should_Send_Webhook_To_Authorized_Current_Tenant_Sync()
        {
            var (tenantId, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebHookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                }));

            AbpSession.TenantId = tenantId;

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Created, data);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_Tenant_Who_Does_Not_Have_Feature_Sync()
        {
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created,
                AppFeatures.WebHookFeature, "true"));

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebHookFeature, "false"));

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Created, new { Name = "Musa", Surname = "Demir" }, subscription.TenantId);

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_Current_Tenant_Who_Does_Not_Have_Feature_Sync()
        {
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Users.Created,
                AppFeatures.WebHookFeature, "true"));

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebHookFeature, "false"));

            AbpSession.TenantId = subscription.TenantId;

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Created, new { Name = "Musa", Surname = "Demir" });

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Tenant_If_All_Required_Features_Granted_Sync()
        {
            //user_deleted webhook requires AppFeatures.WebHookFeature, AppFeatures.TestFeature but not requires all

            var (tenantId, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebHookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                }));

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Deleted, data, tenantId);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var (tenantId2, data2, predicate2) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, data2, tenantId2);

            _backgroundJobManagerSubstitute.Received()
               .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate2(w)));
        }

        [Fact]
        public void Should_Send_Webhook_To_Current_Tenant_If_All_Required_Features_Granted_Sync()
        {
            //user_deleted webhook requires AppFeatures.WebHookFeature, AppFeatures.TestFeature but not requires all

            var (tenantId, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebHookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"}
                }));

            AbpSession.TenantId = tenantId;
            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Deleted, data);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var (tenantId2, data2, predicate2) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            AbpSession.TenantId = tenantId;
            _webhookPublisher.Publish(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, data2, tenantId2);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate2(w)));
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_If_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Sync()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
               new Dictionary<string, string>
               {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
               }));

            //remove one feature
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebHookFeature, "false"));

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, new { Name = "Musa", Surname = "Demir" }, subscription.TenantId);

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_If_Current_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Sync()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebHookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebHookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            AbpSession.TenantId = subscription.TenantId;
            //remove one feature
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.WebHookFeature, "false"));

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Theme.DefaultThemeChanged, new { Name = "Musa", Surname = "Demir" }, AbpSession.TenantId);

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Any<WebHookSenderInput>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Host_If_Subscribed_Sync()
        {
            var subscription = new WebHookSubscription
            {
                TenantId = null,
                Secret = "secret",
                WebHookUri = "www.mywebhook.com",
                WebHookDefinitions = new List<string>() { AppWebHookDefinitionNames.Users.Created },
                Headers = new Dictionary<string, string>
                {
                    { "Key","Value"}
                }
            };

            var webHookSubscriptionManager = Resolve<IWebHookSubscriptionManager>();
            var webHooksConfiguration = Resolve<IWebHooksConfiguration>();

            webHookSubscriptionManager.AddOrUpdateSubscription(subscription);

            var data = new { Name = "Musa", Surname = "Demir" };

            Predicate<WebHookSenderInput> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith("whs_");
                w.WebHookDefinition.ShouldContain(AppWebHookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebHookSubscriptionId.ShouldBe(subscription.Id);
                w.Data.ShouldBe(
                    webHooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webHooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            _webhookPublisher.Publish(AppWebHookDefinitionNames.Users.Created, data, null);

            _backgroundJobManagerSubstitute.Received()
               .Enqueue<WebHookSenderJob, WebHookSenderInput>(Arg.Is<WebHookSenderInput>(w => predicate(w)));
        }
        #endregion
    }
}
