using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Abp.Webhooks;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class TestWebhookSender : DefaultWebhookSender
    {
        public TestWebhookSender(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookManager webhookManager) :
            base(webhooksConfiguration, webhookManager)
        {
        }

        protected override async Task<(bool isSucceed, HttpStatusCode statusCode, string content)> SendHttpRequest(
            HttpRequestMessage request)
        {
            var requestContent = await request.Content.ReadAsStringAsync();
            return (true, HttpStatusCode.OK, requestContent);
        }
    }
}
