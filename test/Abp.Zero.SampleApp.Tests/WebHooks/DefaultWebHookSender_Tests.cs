using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Json;
using Abp.WebHooks;
using Abp.Zero.SampleApp.Application;
using Newtonsoft.Json;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.WebHooks
{
    public class DefaultWebHookSender_Tests : DefaultWebHookSender
    {
        public DefaultWebHookSender_Tests()
            : base(Substitute.For<IWebHooksConfiguration>())
        {
            WebhookSendAttemptStore = Substitute.For<IWebhookSendAttemptStore>();
            WebhookSendAttemptStore.GetSendAttemptCountAsync(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(0));
            WebhookSendAttemptStore.GetSendAttemptCount(Arg.Any<int?>(), Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(0);
        }

        [Fact]
        public async Task GetWebhookPayloadAsync_Tests()
        {
            string data = new { Test = "test" }.ToJsonString();

            var payload = await GetWebhookPayloadAsync(new WebHookSenderInput()
            {
                TenantId = 1,
                WebHookDefinition = AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data
            });

            payload.Event.ShouldBe(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            ((string)JsonConvert.SerializeObject(payload.Data)).ShouldBe(data);
            payload.Attempt.ShouldBe(1);
        }

        [Fact]
        public void GetWebhookPayload_Tests()
        {
            string data = new { Test = "test" }.ToJsonString();

            var payload = GetWebhookPayload(new WebHookSenderInput()
            {
                TenantId = 1,
                WebHookDefinition = AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data
            });

            payload.Event.ShouldBe(AppWebHookDefinitionNames.Theme.DefaultThemeChanged);
            ((string)JsonConvert.SerializeObject(payload.Data)).ShouldBe(data);
            payload.Attempt.ShouldBe(1);
        }


        [Fact]
        public async Task SignWebHookRequest_Tests()
        {
            var payload = await GetWebhookPayloadAsync(new WebHookSenderInput()
            {
                TenantId = 1,
                WebHookDefinition = AppWebHookDefinitionNames.Theme.DefaultThemeChanged,
                Data = new { Test = "test" }.ToJsonString()
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "www.test.com");

            SignWebHookRequest(request, payload.ToJsonString(), "mysecret");

            request.Content.ShouldBeAssignableTo(typeof(StringContent));

            var content = await request.Content.ReadAsStringAsync();
            content.ShouldBe(payload.ToJsonString());

            request.Headers.GetValues(SignatureHeaderName).Single()
                .ShouldBe("sha256=3C-06-C6-E1-30-39-F0-5E-51-27-66-39-36-49-25-21-4D-01-AF-76-FC-D3-4B-14-F4-1E-8F-57-C7-F7-CD-A4");
        }
    }
}
