using System;

namespace Abp.Webhooks
{
    public class WebhookPayload
    {
        public string Id { get; set; }

        public string Event { get; set; }

        public int Attempt { get; set; }

        public dynamic Data { get; set; }

        public DateTime CreationTime { get; set; }

        public WebhookPayload(string id, string webHookEvent, int attempt)
        {
            Id = id;
            Event = webHookEvent;
            Attempt = attempt;
            CreationTime = DateTime.UtcNow;
        }
    }
}
