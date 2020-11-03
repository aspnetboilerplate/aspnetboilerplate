namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyDefinitionContext : IDynamicEntityPropertyDefinitionContext
    {
        public IDynamicEntityPropertyDefinitionManager Manager { get; set; }

        public DynamicEntityPropertyDefinitionContext()
        {
            Manager = NullDynamicEntityPropertyDefinitionManager.Instance;
        }
    }
}
