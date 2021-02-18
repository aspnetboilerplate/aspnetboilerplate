using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.BackgroundJobs;
using Abp.Json;
using Abp.Threading;
using Abp.Webhooks;
using Abp.Webhooks.BackgroundWorker;
using Abp.Zero.SampleApp.Application;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class WebhookPublisher_Tests : WebhookTestBase
    {
        private readonly IWebhookPublisher _webhookPublisher;
        private readonly IBackgroundJobManager _backgroundJobManagerSubstitute;

        public WebhookPublisher_Tests()
        {
            AbpSession.UserId = 1;
            AbpSession.TenantId = null;

            _backgroundJobManagerSubstitute = RegisterFake<IBackgroundJobManager>();
            _webhookPublisher = Resolve<IWebhookPublisher>();
        }

        #region Async

        private Task<(WebhookSubscription subscription, object data, Predicate<WebhookSenderArgs> predicate)> InitializeTestCase(string webhookDefinition)
        {
            return InitializeTestCase(webhookDefinition, null);
        }

        /// <summary>
        /// Creates tenant with adding feature(s), then creates predicate for WebhookSenderArgs which publisher should send to WebhookSenderJob
        /// </summary>
        /// <param name="webhookDefinition"></param>
        /// <param name="tenantFeatures"></param>
        /// <returns></returns>
        private async Task<(WebhookSubscription subscription, object data, Predicate<WebhookSenderArgs> predicate)> InitializeTestCase(string webhookDefinition, Dictionary<string, string> tenantFeatures)
        {
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(webhookDefinition, tenantFeatures);

            AbpSession.TenantId = subscription.TenantId;

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(webhookDefinition);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebhookSubscriptionId.ShouldBe(subscription.Id);
                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            return (subscription, data, predicate);
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_If_There_Is_No_Subscription_Async()
        {
            await CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Users.Created,
                AppFeatures.WebhookFeature, "true");

            AbpSession.TenantId = GetDefaultTenant().Id;

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Test,
                new
                {
                    Name = "Musa",
                    Surname = "Demir"
                });

            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Authorized_Tenant_Async()
        {
            var (subscription, data, predicate) = await InitializeTestCase(AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                });

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Created, data, subscription.TenantId);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Authorized_Current_Tenant_Async()
        {
            var (subscription, data, predicate) = await InitializeTestCase(AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                });

            AbpSession.TenantId = subscription.TenantId;

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Created, data);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_Tenant_If_Features_Are_Not_Granted_Async()
        {
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Users.Created,
                AppFeatures.WebhookFeature, "true");

            await AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebhookFeature, "false");

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Created, new {Name = "Musa", Surname = "Demir"}, subscription.TenantId);

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_Current_Tenant_If_Features_Are_Not_Granted_Async()
        {
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Users.Created,
                AppFeatures.WebhookFeature, "true");

            AbpSession.TenantId = subscription.TenantId;

            await AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.WebhookFeature, "false");

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Created, new {Name = "Musa", Surname = "Demir"});

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Tenant_If_All_Required_Features_Granted_Async()
        {
            //user_deleted webhook requires AppFeatures.WebhookFeature, AppFeatures.TestFeature but not requires all
            var (subscription, data, predicate) = await InitializeTestCase(AppWebhookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                });

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Deleted, data, subscription.TenantId);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var (subscription2, data2, predicate2) = await InitializeTestCase(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, data2, subscription2.TenantId);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate2(w)));
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Current_Tenant_If_All_Required_Features_Granted_Async()
        {
            //user_deleted webhook requires AppFeatures.WebhookFeature, AppFeatures.TestFeature but not requires all
            var (subscription, data, predicate) = await InitializeTestCase(AppWebhookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                });

            AbpSession.TenantId = subscription.TenantId;
            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Deleted, data);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var (subscription2, data2, predicate2) = await InitializeTestCase(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            AbpSession.TenantId = subscription2.TenantId;
            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, data2);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate2(w)));
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_If_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Async()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            //remove one feature
            await AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebhookFeature, "false");

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, new {Name = "Musa", Surname = "Demir"}, subscription.TenantId);

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public async Task Should_Not_Send_Webhook_To_If_Current_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Async()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = await CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                });

            //remove one feature
            await AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebhookFeature, "false");

            AbpSession.TenantId = subscription.TenantId;
            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, new {Name = "Musa", Surname = "Demir"});

            //should not try to send
            await _backgroundJobManagerSubstitute.DidNotReceive().EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Host_If_Subscribed_Async()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = null,
                Secret = "secret",
                WebhookUri = "www.mywebhook.com",
                Webhooks = new List<string>() {AppWebhookDefinitionNames.Users.Created},
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value"}
                }
            };

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebhookSubscriptionId.ShouldBe(subscription.Id);
                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Users.Created, data, null);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));
        }

        [Fact]
        public async Task Should_Deactivate_Subscription_If_Reached_To_Max_Fail_Count_Async()
        {
            var (subscription, data, predicate) = await InitializeTestCase(AppWebhookDefinitionNames.Test);

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();
            webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled = true;
            webhooksConfiguration.MaxConsecutiveFailCountBeforeDeactivateSubscription = 1;

            var webhookSenderSubstitute = RegisterFake<IWebhookSender>();
            webhookSenderSubstitute.When(w => w.SendWebhook(Arg.Any<WebhookSenderArgs>()))
                .Do(x =>
                    throw new Exception()
                );

            var webhookSendAttemptStore = RegisterFake<IWebhookSendAttemptStore>();
            webhookSendAttemptStore
                .HasXConsecutiveFail(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<int>())
                .Returns(true);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            _backgroundJobManagerSubstitute.When(m => m.EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>()))
                .Do((callback) =>
                {
                    var args = callback.Args();
                    args[0].ShouldBeAssignableTo<WebhookSenderArgs>("Argument is not WebhookSenderArgs");

                    var webhookSenderJob = Resolve<WebhookSenderJob>();
                    webhookSenderJob.Execute(args[0] as WebhookSenderArgs);

                    var sub = webhookSubscriptionManager.Get(subscription.Id);
                    sub.IsActive.ShouldBeFalse(); //after it get error MaxConsecutiveFailCountBeforeDeactivateSubscription times(in our case it is 1) subscription becomes deactivate
                });

            await webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(subscription);

            var storedSubscription = webhookSubscriptionManager.Get(subscription.Id);
            storedSubscription.IsActive.ShouldBeTrue(); //subscription is active 

            await _webhookPublisher.PublishAsync(AppWebhookDefinitionNames.Test, data, subscription.TenantId);

            await _backgroundJobManagerSubstitute.Received()
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));

            webhookSenderSubstitute.Received().SendWebhook(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Subscribed_Tenants_Async()
        {
            var tenant1Subscription = await CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            );

            var tenant2Subscription = await CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            );

            var tenant3Subscription = await CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Test,
                null
            );

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                var acceptedIds = new[] {tenant1Subscription.Id, tenant2Subscription.Id}; //since tenant 3 is not subscribed to that event, tenant3's subscription should not receive that event
                acceptedIds.ShouldContain(w.WebhookSubscriptionId);

                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            //Even if we try to publish this webhook to tenant 3, it shouldn't get it since it has no subscription to that AppWebhookDefinitionNames.Users.Created
            await _webhookPublisher.PublishAsync(new[]
            {
                tenant1Subscription.TenantId,
                tenant2Subscription.TenantId,
                tenant3Subscription.TenantId
            }, AppWebhookDefinitionNames.Users.Created, data);

            await _backgroundJobManagerSubstitute.Received(2)
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(p => predicate(p)));
        }

        [Fact]
        public async Task Should_Send_Webhook_To_Only_Listed_Subscribed_Tenants_Async()
        {
            var tenant1Subscription = await CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            );

            var tenant2Subscription = await CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            );

            var tenant3Subscription = await CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Test,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            );

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                var acceptedIds = new[] {tenant1Subscription.Id, tenant2Subscription.Id}; //since tenant 3 is not listed in publish function, tenant3's subscription should not receive that event
                acceptedIds.ShouldContain(w.WebhookSubscriptionId);

                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            //only send to tenant1 and tenant2, not tenant3
            await _webhookPublisher.PublishAsync(new[]
            {
                tenant1Subscription.TenantId,
                tenant2Subscription.TenantId
            }, AppWebhookDefinitionNames.Users.Created, data);

            await _backgroundJobManagerSubstitute.Received(2)
                .EnqueueAsync<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(p => predicate(p)));
        }

        #endregion

        #region Sync

        [Fact]
        public void Should_Not_Send_Webhook_If_There_Is_Subscription_Sync()
        {
            AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Users.Created,
                AppFeatures.WebhookFeature, "true"));

            AbpSession.TenantId = GetDefaultTenant().Id;
            _webhookPublisher.Publish(AppWebhookDefinitionNames.Test,
                new
                {
                    Name = "Musa",
                    Surname = "Demir"
                });

            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Authorized_Tenant_Sync()
        {
            var (subscription, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }));

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Created, data, subscription.TenantId);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));
        }

        [Fact]
        public void Should_Send_Webhook_To_Authorized_Current_Tenant_Sync()
        {
            var (subscription, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }));

            AbpSession.TenantId = subscription.TenantId;

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Created, data);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_Tenant_Who_Does_Not_Have_Feature_Sync()
        {
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Users.Created,
                AppFeatures.WebhookFeature, "true"));

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebhookFeature, "false"));

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Created, new {Name = "Musa", Surname = "Demir"}, subscription.TenantId);

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_Current_Tenant_Who_Does_Not_Have_Feature_Sync()
        {
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Users.Created,
                AppFeatures.WebhookFeature, "true"));

            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebhookFeature, "false"));

            AbpSession.TenantId = subscription.TenantId;

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Created, new {Name = "Musa", Surname = "Demir"});

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Tenant_If_All_Required_Features_Granted_Sync()
        {
            //user_deleted webhook requires AppFeatures.WebhookFeature, AppFeatures.TestFeature but not requires all

            var (subscription, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }));

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Deleted, data, subscription.TenantId);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var (subscription2, data2, predicate2) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, data2, subscription2.TenantId);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate2(w)));
        }

        [Fact]
        public void Should_Send_Webhook_To_Current_Tenant_If_All_Required_Features_Granted_Sync()
        {
            //user_deleted webhook requires AppFeatures.WebhookFeature, AppFeatures.TestFeature but not requires all

            var (subscription, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Users.Deleted,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }));

            AbpSession.TenantId = subscription.TenantId;
            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Deleted, data);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));

            _backgroundJobManagerSubstitute.ClearReceivedCalls();

            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var (subscription2, data2, predicate2) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            AbpSession.TenantId = subscription2.TenantId;
            _webhookPublisher.Publish(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, data2, subscription2.TenantId);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate2(w)));
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_If_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Sync()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            //remove one feature
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(subscription.TenantId.Value, AppFeatures.WebhookFeature, "false"));

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, new {Name = "Musa", Surname = "Demir"}, subscription.TenantId);

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public void Should_Not_Send_Webhook_To_If_Current_Tenant_Does_Not_Have_All_Features_When_Its_Required_All_Sync()
        {
            //DefaultThemeChanged webhook requires AppFeatures.WebhookFeature, AppFeatures.ThemeFeature and requires all
            var subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"},
                    {AppFeatures.ThemeFeature, "true"}
                }));

            AbpSession.TenantId = subscription.TenantId;
            //remove one feature
            AsyncHelper.RunSync(() => AddOrReplaceFeatureToTenantAsync(AbpSession.TenantId.Value, AppFeatures.WebhookFeature, "false"));

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Theme.DefaultThemeChanged, new {Name = "Musa", Surname = "Demir"}, AbpSession.TenantId);

            //should not try to send
            _backgroundJobManagerSubstitute.DidNotReceive().Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Host_If_Subscribed_Sync()
        {
            var subscription = new WebhookSubscription
            {
                TenantId = null,
                Secret = "secret",
                WebhookUri = "www.mywebhook.com",
                Webhooks = new List<string>() {AppWebhookDefinitionNames.Users.Created},
                Headers = new Dictionary<string, string>
                {
                    {"Key", "Value"}
                }
            };

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            webhookSubscriptionManager.AddOrUpdateSubscription(subscription);

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                w.WebhookSubscriptionId.ShouldBe(subscription.Id);
                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Users.Created, data, null);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));
        }

        [Fact]
        public void Should_Deactivate_Subscription_If_Reached_To_Max_Fail_Count_Sync()
        {
            var (subscription, data, predicate) = AsyncHelper.RunSync(() => InitializeTestCase(AppWebhookDefinitionNames.Test));

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();
            webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled = true;
            webhooksConfiguration.MaxConsecutiveFailCountBeforeDeactivateSubscription = 1;

            var webhookSenderSubstitute = RegisterFake<IWebhookSender>();
            webhookSenderSubstitute.When(w => w.SendWebhook(Arg.Any<WebhookSenderArgs>()))
                .Do(x =>
                    throw new Exception()
                );

            var webhookSendAttemptStore = RegisterFake<IWebhookSendAttemptStore>();
            webhookSendAttemptStore
                .HasXConsecutiveFail(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<int>())
                .Returns(true);

            var webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();

            _backgroundJobManagerSubstitute.When(m => m.Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Any<WebhookSenderArgs>()))
                .Do(callback =>
                {
                    var args = callback.Args();
                    args[0].ShouldBeAssignableTo<WebhookSenderArgs>("Argument is not WebhookSenderArgs");

                    var webhookSenderJob = Resolve<WebhookSenderJob>();
                    webhookSenderJob.Execute(args[0] as WebhookSenderArgs);

                    var sub = webhookSubscriptionManager.Get(subscription.Id);
                    sub.IsActive.ShouldBeFalse(); //after it get error MaxConsecutiveFailCountBeforeDeactivateSubscription times(in our case it is 1) subscription becomes deactivate
                });

            webhookSubscriptionManager.AddOrUpdateSubscription(subscription);

            var storedSubscription = webhookSubscriptionManager.Get(subscription.Id);
            storedSubscription.IsActive.ShouldBeTrue(); //subscription is active 

            _webhookPublisher.Publish(AppWebhookDefinitionNames.Test, data, subscription.TenantId);

            _backgroundJobManagerSubstitute.Received()
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(w => predicate(w)));

            webhookSenderSubstitute.Received().SendWebhook(Arg.Any<WebhookSenderArgs>());
        }

        [Fact]
        public void Should_Send_Webhook_To_Subscribed_Tenants()
        {
            var tenant1Subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            ));

            var tenant2Subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            ));

            var tenant3Subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Test,
                null
            ));

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                var acceptedIds = new[] {tenant1Subscription.Id, tenant2Subscription.Id}; //since tenant 3 is not subscribed to that event, tenant3's subscription should not receive that event
                acceptedIds.ShouldContain(w.WebhookSubscriptionId);

                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            //Even if we try to publish this webhook to tenant 3, it shouldn't get it since it has no subscription to that AppWebhookDefinitionNames.Users.Created
            _webhookPublisher.Publish(new[]
            {
                tenant1Subscription.TenantId,
                tenant2Subscription.TenantId,
                tenant3Subscription.TenantId
            }, AppWebhookDefinitionNames.Users.Created, data);

            _backgroundJobManagerSubstitute.Received(2)
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(p => predicate(p)));
        }

        [Fact]
        public void Should_Send_Webhook_To_Only_Listed_Subscribed_Tenants()
        {
            var tenant1Subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            ));

            var tenant2Subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Users.Created,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            ));

            var tenant3Subscription = AsyncHelper.RunSync(() => CreateTenantAndSubscribeToWebhookAsync(
                AppWebhookDefinitionNames.Test,
                new Dictionary<string, string>
                {
                    {AppFeatures.WebhookFeature, "true"}
                }
            ));

            var webhooksConfiguration = Resolve<IWebhooksConfiguration>();

            var data = new {Name = "Musa", Surname = "Demir"};

            Predicate<WebhookSenderArgs> predicate = w =>
            {
                w.Secret.ShouldNotBeNullOrEmpty();
                w.Secret.ShouldStartWith(WebhookSubscriptionSecretPrefix);
                w.WebhookName.ShouldContain(AppWebhookDefinitionNames.Users.Created);

                w.Headers.Count.ShouldBe(1);
                w.Headers.Single().Key.ShouldBe("Key");
                w.Headers.Single().Value.ShouldBe("Value");

                var acceptedIds = new[] {tenant1Subscription.Id, tenant2Subscription.Id}; //since tenant 3 is not listed in publish function, tenant3's subscription should not receive that event
                acceptedIds.ShouldContain(w.WebhookSubscriptionId);

                w.Data.ShouldBe(
                    webhooksConfiguration.JsonSerializerSettings != null
                        ? data.ToJsonString(webhooksConfiguration.JsonSerializerSettings)
                        : data.ToJsonString()
                );
                return true;
            };

            //only send to tenant1 and tenant2, not tenant3
            _webhookPublisher.Publish(new[]
            {
                tenant1Subscription.TenantId,
                tenant2Subscription.TenantId
            }, AppWebhookDefinitionNames.Users.Created, data);

            _backgroundJobManagerSubstitute.Received(2)
                .Enqueue<WebhookSenderJob, WebhookSenderArgs>(Arg.Is<WebhookSenderArgs>(p => predicate(p)));
        }

        #endregion
    }
}