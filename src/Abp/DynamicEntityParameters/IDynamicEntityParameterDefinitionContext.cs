namespace Abp.DynamicEntityParameters
{
    public interface IDynamicEntityParameterDefinitionContext
    {
        /// <summary>
        /// Gets the webhook definition manager.
        /// </summary>
        IDynamicEntityParameterDefinitionManager Manager { get; set; }
    }
}
