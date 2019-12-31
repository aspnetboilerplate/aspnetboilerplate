namespace Abp.WebHooks
{
    public class WebhookBody
    {
        public string Event { get; set; }

        public int Attempt { get; set; }

        public object Data { get; set; }
    }
}
