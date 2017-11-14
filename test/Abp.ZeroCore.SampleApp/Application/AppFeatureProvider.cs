using Abp.Application.Features;
using Abp.UI.Inputs;

using static Abp.ZeroCore.SampleApp.Application.AppLocalizationHelper;

namespace Abp.ZeroCore.SampleApp.Application
{
    public class AppFeatureProvider : FeatureProvider
    {
        public override void SetFeatures(IFeatureDefinitionContext context)
        {
            context.Create(
                AppFeatures.SimpleBooleanFeature,
                defaultValue: "false",
                displayName: L("SimpleBooleanFeature"),
                inputType: new CheckboxInputType()
            );

            context.Create(
                AppFeatures.SimpleIntFeature,
                defaultValue: "0",
                displayName: L("SimpleIntFeature")
            );
        }
    }
}
