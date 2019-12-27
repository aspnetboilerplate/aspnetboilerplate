using Abp.Dependency;

namespace Abp.WebHooks
{
    public abstract class WebHookDefinitionProvider : ITransientDependency
    {
        /// <summary>
        /// Used to add/manipulate webhook definitions.
        /// </summary>
        /// <param name="context">Context</param>,
        public abstract void SetWebHooks(IWebHookDefinitionContext context);
    }
}
