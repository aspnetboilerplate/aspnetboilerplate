namespace Abp.DynamicEntityParameters
{
    public interface IDynamicEntityParameterDefinitionContext
    {
        /// <summary>
        /// Gets the DynamicEntityParameter definition manager.
        /// </summary>
        IDynamicEntityParameterDefinitionManager Manager { get; set; }
    }
}
