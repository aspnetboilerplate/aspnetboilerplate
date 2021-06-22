using System;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Webhooks;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class WebhookSender_Tests : WebhookTestBase
    {
        private IWebhookManager _webhookManager;
        private readonly IWebhookSender _webhookSender;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;
        private readonly IWebhookEventStore _webhookEventStore;
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;

        public WebhookSender_Tests()
        {
            _webhookManager = Resolve<IWebhookManager>();
            _webhookSender = Resolve<IWebhookSender>();
            _webhookSubscriptionManager = Resolve<IWebhookSubscriptionManager>();
            _webhookEventStore = Resolve<IWebhookEventStore>();
            _webhookSendAttemptStore = Resolve<IWebhookSendAttemptStore>();
        }

        [Fact]
        public async Task Should_Throw_Exception_Async()
        {
            await _webhookSender
                .SendWebhookAsync(new WebhookSenderArgs())
                .ShouldThrowAsync<ArgumentNullException>();

            var exception = await _webhookSender
                .SendWebhookAsync(new WebhookSenderArgs
                {
                    WebhookEventId = Guid.NewGuid(),
                    WebhookSubscriptionId = Guid.Empty
                })
                .ShouldThrowAsync<ArgumentNullException>();

            exception.Message.ShouldContain(nameof(WebhookSenderArgs.WebhookSubscriptionId));

            var exception2 = await _webhookSender
                .SendWebhookAsync(new WebhookSenderArgs()
                {
                    WebhookEventId = Guid.Empty,
                    WebhookSubscriptionId = Guid.NewGuid()
                })
                .ShouldThrowAsync<ArgumentNullException>();

            exception2.Message.ShouldContain(nameof(WebhookSenderArgs.WebhookEventId));
        }

        [Fact]
        public async Task Attempt_Count_Test()
        {
            // Arrange
            var secret = "secret";

            var webhookSubscription = new WebhookSubscription()
            {
                TenantId = AbpSession.TenantId,
                Secret = secret,
                WebhookUri = "www.test.com"
            };

            await _webhookSubscriptionManager.AddOrUpdateSubscriptionAsync(webhookSubscription);

            var webhookEventId = await _webhookEventStore.InsertAndGetIdAsync(new WebhookEvent()
            {
                TenantId = AbpSession.TenantId,
                Data = "{}",
                WebhookName = "test"
            });

            // Act
            var sendAttemptId = await _webhookSender.SendWebhookAsync(new WebhookSenderArgs
            {
                TenantId = AbpSession.TenantId,
                WebhookName = "test",
                WebhookUri = "www.test.com",
                WebhookSubscriptionId = webhookSubscription.Id,
                Data = "{}",
                Secret = secret,
                WebhookEventId = webhookEventId
            });

            // Assert
            var sendAttempt = await _webhookSendAttemptStore.GetAsync(AbpSession.TenantId, sendAttemptId);
            var response = sendAttempt.Response.FromJsonString<WebhookPayload>();
            response.Attempt.ShouldBe(1);
        }
    }
}
