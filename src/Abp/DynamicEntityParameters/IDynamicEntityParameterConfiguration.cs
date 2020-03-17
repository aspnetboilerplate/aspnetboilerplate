using Abp.Collections;

namespace Abp.DynamicEntityParameters
{
    public interface IDynamicEntityParameterConfiguration
    {
        ITypeList<DynamicEntityParameterDefinitionProvider> Providers { get; }
    }
}
