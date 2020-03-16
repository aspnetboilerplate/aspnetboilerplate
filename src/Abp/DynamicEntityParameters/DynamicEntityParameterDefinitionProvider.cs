using Abp.Dependency;

namespace Abp.DynamicEntityParameters
{
    public abstract class DynamicEntityParameterDefinitionProvider : ITransientDependency
    {
        /// <summary>
        /// Used to add/manipulate dynamic parameter definitions.
        /// </summary>
        /// <param name="context">Context</param>,
        public abstract void SetDynamicEntityParameters(IDynamicEntityParameterDefinitionContext context);
    }
}
