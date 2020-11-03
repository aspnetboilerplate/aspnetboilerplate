using Abp.Collections;

namespace Abp.DynamicEntityProperties
{
    public interface IDynamicEntityPropertyConfiguration
    {
        ITypeList<DynamicEntityPropertyDefinitionProvider> Providers { get; }
    }
}
