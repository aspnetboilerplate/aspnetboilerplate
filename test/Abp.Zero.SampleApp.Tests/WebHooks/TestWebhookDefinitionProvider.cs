using Abp.Application.Features;
using Abp.Localization;
using Abp.Webhooks;
using Abp.Zero.SampleApp.Application;

namespace Abp.Zero.SampleApp.Tests.Webhooks
{
    public class TestWebhookDefinitionProvider : WebhookDefinitionProvider
    {
        public override void SetWebhooks(IWebhookDefinitionContext context)
        {
            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Test,
                    L("TestWebhook"),
                    L("DefaultDescription")
                ));

            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Users.Created,
                    L("UserCreatedWebhook"),
                    L("DescriptionCreated"),
                    new SimpleFeatureDependency(AppFeatures.WebhookFeature)
                ));

            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Users.Deleted,
                    L("DeletedDeletedWebhook"),
                    L("DescriptionDeletedDeleted"),
                    new SimpleFeatureDependency(false, AppFeatures.WebhookFeature, AppFeatures.TestFeature)
                ));

            context.Manager.Add(
                new WebhookDefinition(
                    AppWebhookDefinitionNames.Theme.DefaultThemeChanged,
                    L("DefaultThemeChanged"),
                    L("TriggersWhenDefaultThemeChanged"),
                    new SimpleFeatureDependency(true, AppFeatures.WebhookFeature, AppFeatures.ThemeFeature)
                ));
        }

        private ILocalizableString L(string name)
        {
            return new LocalizableString(name, AbpZeroConsts.LocalizationSourceName);
        }
    }
}