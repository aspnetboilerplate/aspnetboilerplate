using System;
using Abp.Extensions;

namespace Abp.Webhooks
{
    public class WebhookPayload
    {
        public string Id { get; set; }

        public string Event { get; set; }

        public int Attempt { get; set; }

        public dynamic Data { get; set; }

        public DateTime CreationTimeUtc { get; set; }

        public WebhookPayload(string id, string webHookEvent, int attempt)
        {
            if (id.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (webHookEvent.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(webHookEvent));
            }

            if (attempt <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attempt));
            }

            Id = id;
            Event = webHookEvent;
            Attempt = attempt;
            CreationTimeUtc = DateTime.UtcNow;
        }
    }
}
