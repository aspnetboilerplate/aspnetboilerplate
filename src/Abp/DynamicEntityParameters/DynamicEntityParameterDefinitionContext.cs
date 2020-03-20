namespace Abp.DynamicEntityParameters
{
    public class DynamicEntityParameterDefinitionContext : IDynamicEntityParameterDefinitionContext
    {
        public IDynamicEntityParameterDefinitionManager Manager { get; set; }

        public DynamicEntityParameterDefinitionContext()
        {
            Manager = NullDynamicEntityParameterDefinitionManager.Instance;
        }
    }
}
