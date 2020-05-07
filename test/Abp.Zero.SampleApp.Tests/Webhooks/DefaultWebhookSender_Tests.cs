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

        //WebhookPayload with parameterless constructor for testing
        public class WebhookPayloadTest : WebhookPayload
        {
            public WebhookPayloadTest(string id, string webHookEvent, int attempt) : base(id, webHookEvent, attempt)
            {
            }

            public WebhookPayloadTest() : base("test", "test", int.MaxValue)
            {

            }
        }

        [Fact]
        public void GetSerializedBody_SendExactSameData_False_Test()
        {
            string data = new { Test = "test" }.ToJsonString();

            var serializedBody = GetSerializedBody(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data,
                SendExactSameData = false
            });

            serializedBody.ShouldContain(data);
            serializedBody.ShouldNotBe(data); // serializedBody must be created with WebhookPayload

            WebhookPayloadTest payload;
            try
            {
                payload = serializedBody.FromJsonString<WebhookPayloadTest>();
            }
            catch (Exception e)
            {
                throw new Exception("Payload must be WebhookPayload json");
            }

            payload.Id.ShouldNotBe("test");
            payload.Event.ShouldBe(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            payload.Attempt.ShouldBe(1);
        }

        [Fact]
        public void GetSerializedBody_SendExactSameData_True_Test()
        {
            string data = new { Test = "test" }.ToJsonString();

            var serializedBody = GetSerializedBody(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data,
                SendExactSameData = true
            });

            serializedBody.ShouldBe(data); // serializedBody must be equal to data
        }


        [Fact]
        public async Task GetSerializedBodyAsync_SendExactSameData_False_Test()
        {
            string data = new { Test = "test" }.ToJsonString();

            var serializedBody = await GetSerializedBodyAsync(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data,
                SendExactSameData = false
            });

            serializedBody.ShouldContain(data);
            serializedBody.ShouldNotBe(data); // serializedBody must be created with WebhookPayload

            WebhookPayloadTest payload;
            try
            {
                payload = serializedBody.FromJsonString<WebhookPayloadTest>();
            }
            catch (Exception e)
            {
                throw new Exception("Payload must be WebhookPayload json");
            }

            payload.Id.ShouldNotBe("test");
            payload.Event.ShouldBe(AppWebhookDefinitionNames.Theme.DefaultThemeChanged);
            payload.Attempt.ShouldBe(1);
        }

        [Fact]
        public async Task GetSerializedBodyAsync_SendExactSameData_True_Test()
        {
            string data = new { Test = "test" }.ToJsonString();

            var serializedBody = await GetSerializedBodyAsync(new WebhookSenderArgs()
            {
                TenantId = 1,
                WebhookName = AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                Data = data,
                SendExactSameData = true
            });

            serializedBody.ShouldBe(data); // serializedBody must be equal to data
        }
    }
}