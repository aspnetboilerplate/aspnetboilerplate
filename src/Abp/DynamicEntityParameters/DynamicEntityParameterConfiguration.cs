using Abp.Collections;

namespace Abp.DynamicEntityParameters
{
    public class DynamicEntityParameterConfiguration : IDynamicEntityParameterConfiguration
    {
        public ITypeList<DynamicEntityParameterDefinitionProvider> Providers { get; }

        public DynamicEntityParameterConfiguration()
        {
            Providers = new TypeList<DynamicEntityParameterDefinitionProvider>();
        }
    }
}
