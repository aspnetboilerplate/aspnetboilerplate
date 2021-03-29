using Abp.DynamicEntityProperties;
using Abp.UI.Inputs;
using Abp.Zero.SampleApp.EntityHistory;

namespace Abp.Zero.SampleApp.Tests.DynamicEntityProperties
{
    public class MyDynamicEntityPropertyDefinitionProvider : DynamicEntityPropertyDefinitionProvider
    {
        public override void SetDynamicEntityProperties(IDynamicEntityPropertyDefinitionContext context)
        {
            context.Manager.AddAllowedInputType<SingleLineStringInputType>();
            context.Manager.AddAllowedInputType<CheckboxInputType>();
            context.Manager.AddAllowedInputType<ComboboxInputType>();

            context.Manager.AddEntity<Country, int>();
        }
    }
}