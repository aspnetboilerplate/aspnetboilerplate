using Abp.Localization;

namespace Abp.Runtime.Validation
{
    public static class CustomValidationContextExtensions
    {
        /// <param name="validationContext">Validation context</param>
        /// <param name="sourceName">Localization source name</param>
        /// <param name="key">Localization key</param>
        public static string Localize(this CustomValidationContext validationContext, string sourceName, string key)
        {
            var localizationManager = validationContext.IocResolver.Resolve<ILocalizationManager>();
            var source = localizationManager.GetSource(sourceName);
            return source.GetString(key);
        }
    }
}