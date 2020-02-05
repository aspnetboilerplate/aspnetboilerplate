using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Json;
using Abp.Webhooks;
using Abp.Zero.SampleApp.Application;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class DefaultWebhookSender_Tests : DefaultWebhookSender
    {
        public DefaultWebhookSender_Tests()
            : base(Substitute.For<IWebhooksConfiguration>())
        {
            WebhookSendAttemptStore = Substitute.For<IWebhookSendAttemptStore>();
            WebhookSendAttemptStore.GetSendAttemptCountAsync(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(0));
            WebhookSendAttemptStore.GetSendAttemptCount(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(0);
        }

        [Fact]
        public async Task GetWebhookPayloadAsync_Tests()
        {
            string data = new { Test = "test" }.ToJsonString();

            var payload = await GetWebhookPayloadAsync(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data
            });

            payload.Event.ShouldBe(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            ((string)JsonConvert.SerializeObject(payload.Data)).ShouldBe(data);
            payload.Attempt.ShouldBe(1);
        }

        [Fact]
        public void GetWebhookPayload_Tests()
        {
            string data = new { Test = "test" }.ToJsonString();

            var payload = GetWebhookPayload(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data
            });

            payload.Event.ShouldBe(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            ((string)JsonConvert.SerializeObject(payload.Data)).ShouldBe(data);
            payload.Attempt.ShouldBe(1);
        }


        [Fact]
        public async Task SignWebhookRequest_Tests()
        {
            var payload = await GetWebhookPayloadAsync(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = new { Test = "test" }.ToJsonString()
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "www.test.com");

            SignWebhookRequest(request, payload.ToJsonString(), "mysecret");

            request.Content.ShouldBeAssignableTo(typeof(StringContent));

            var content = await request.Content.ReadAsStringAsync();
            content.ShouldBe(payload.ToJsonString());

            request.Headers.GetValues(SignatureHeaderName).Single()
                .ShouldStartWith("sha256=");
        }
    }
}
