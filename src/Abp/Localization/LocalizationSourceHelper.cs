using Adorable.Configuration.Startup;
using Adorable.Logging;

namespace Adorable.Localization
{
    public static class LocalizationSourceHelper
    {
        public static string ReturnGivenNameOrThrowException(ILocalizationConfiguration configuration, string sourceName, string name)
        {
            var exceptionMessage = string.Format(
                "Can not find '{0}' in localization source '{1}'!",
                name, sourceName
                );

            if (!configuration.ReturnGivenTextIfNotFound)
            {
                throw new AbpException(exceptionMessage);
            }

            LogHelper.Logger.Warn(exceptionMessage);

            return configuration.WrapGivenTextIfNotFound
                ? string.Format("[{0}]", name)
                : name;
        }
    }
}
