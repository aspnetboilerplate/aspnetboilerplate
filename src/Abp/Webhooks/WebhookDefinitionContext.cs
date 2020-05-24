namespace Abp.Webhooks
{
    public class WebhookDefinitionContext : IWebhookDefinitionContext
    {
        public IWebhookDefinitionManager Manager { get; private set; }
        
        public WebhookDefinitionContext(IWebhookDefinitionManager manager)
        {
            Manager = manager;
        }
    }
}
