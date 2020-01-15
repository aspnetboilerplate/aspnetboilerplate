using System;

namespace Abp.Webhooks
{
    public class WebhookPayload
    {
        public string Event { get; set; }

        public int Attempt { get; set; }

        public dynamic Data { get; set; }

        public DateTime CreationTime { get; set; }

        public string IdempotencyKey { get; set; }

        public WebhookPayload()
        {
            CreationTime = DateTime.UtcNow;
            IdempotencyKey = Guid.NewGuid().ToString();
        }
    }
}
