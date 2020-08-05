using Abp.Collections;

namespace Abp.DynamicEntityProperties
{
    public class DynamicEntityPropertyConfiguration : IDynamicEntityPropertyConfiguration
    {
        public ITypeList<DynamicEntityPropertyDefinitionProvider> Providers { get; }

        public DynamicEntityPropertyConfiguration()
        {
            Providers = new TypeList<DynamicEntityPropertyDefinitionProvider>();
        }
    }
}
