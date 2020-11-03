namespace Abp.Webhooks
{
    public interface IWebhookDefinitionContext
    {
        /// <summary>
        /// Gets the webhook definition manager.
        /// </summary>
        IWebhookDefinitionManager Manager { get; }
    }
}
