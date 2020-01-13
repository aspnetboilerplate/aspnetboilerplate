namespace Abp.Webhooks
{
    public class WebhookPayload
    {
        public string Event { get; set; }

        public int Attempt { get; set; }

        public dynamic Data { get; set; }
    }
}
