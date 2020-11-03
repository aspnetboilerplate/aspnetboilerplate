using Abp.Dependency;

namespace Abp.DynamicEntityProperties
{
    public abstract class DynamicEntityPropertyDefinitionProvider : ITransientDependency
    {
        /// <summary>
        /// Used to add/manipulate dynamic property definitions.
        /// </summary>
        /// <param name="context">Context</param>,
        public abstract void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context);
    }
}
