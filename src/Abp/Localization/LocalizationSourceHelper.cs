using System.Globalization;
using Abp.Configuration.Startup;
using Abp.Extensions;
using Abp.Logging;

namespace Abp.Localization
{
    public static class LocalizationSourceHelper
    {
        public static string ReturnGivenNameOrThrowException(ILocalizationConfiguration configuration, string sourceName, string name, CultureInfo culture)
        {
            var exceptionMessage = $"Can not find '{name}' in localization source '{sourceName}'!";

            if (!configuration.ReturnGivenTextIfNotFound)
            {
                throw new AbpException(exceptionMessage);
            }

            LogHelper.Logger.Warn(exceptionMessage);

#if NET46
            var notFoundText = configuration.HumanizeTextIfNotFound
                ? name.ToSentenceCase(culture)
                : name;
#else
            var notFoundText = configuration.HumanizeTextIfNotFound
                ? name.ToSentenceCase() //TODO: Removed culture since it's not supported by netstandard
                : name;
#endif

            return configuration.WrapGivenTextIfNotFound
                ? $"[{notFoundText}]"
                : notFoundText;
        }
    }
}
