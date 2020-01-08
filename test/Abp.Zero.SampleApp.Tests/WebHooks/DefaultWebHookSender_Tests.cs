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
            WebHookWorkItemStore = Substitute.For<IWebHookWorkItemStore>();
            WebHookWorkItemStore.GetRepetitionCountAsync(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(Task.FromResult(0));
            WebHookWorkItemStore.GetRepetitionCount(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(0);
        }

        [Fact]
        public async Task GetWebhookBodyAsync_Tests()
        {
            string data = new { Test = "test" }.ToJsonString();

            var webHookBody = await GetWebhookBodyAsync(new WebHookSenderInput()
            {
                WebHookDefinition = AppWebHookDefinitionNames.Chat.NewMessageReceived,
                Data = data
            });

            webHookBody.Event.ShouldBe(AppWebHookDefinitionNames.Chat.NewMessageReceived);
            ((string)JsonConvert.SerializeObject(webHookBody.Data)).ShouldBe(data);
            webHookBody.Attempt.ShouldBe(1);
        }

        [Fact]
        public async Task SignWebHookRequest_Tests()
        {
            var webHookBody = await GetWebhookBodyAsync(new WebHookSenderInput()
            {
                WebHookDefinition = AppWebHookDefinitionNames.Chat.NewMessageReceived,
                Data = new { Test = "test" }.ToJsonString()
            });

            var request = new HttpRequestMessage(HttpMethod.Post, "www.test.com");

            SignWebHookRequest(request, webHookBody.ToJsonString(), "mysecret");

            request.Content.ShouldBeAssignableTo(typeof(StringContent));

            var content = await request.Content.ReadAsStringAsync();
            content.ShouldBe(webHookBody.ToJsonString());

            request.Headers.GetValues(SignatureHeaderName).Single()
                .ShouldBe("sha256=75-EF-72-4A-9B-AB-64-B6-F3-31-61-59-DB-BF-D5-2A-9F-C8-F5-F9-0B-BA-77-5D-91-DB-B9-86-42-A1-CE-C0");
        }
    }
}
