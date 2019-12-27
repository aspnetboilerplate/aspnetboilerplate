namespace Abp.WebHooks
{
    public interface IWebHookDefinitionContext
    {
        /// <summary>
        /// Gets the webhook definition manager.
        /// </summary>
        IWebHookDefinitionManager Manager { get; }
    }
}
