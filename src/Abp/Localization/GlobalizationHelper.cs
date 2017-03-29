using System.Globalization;
using Abp.Extensions;

namespace Abp.Localization
{
    internal static class GlobalizationHelper
    {
        public static bool IsValidCultureCode(string cultureCode)
        {
            if (cultureCode.IsNullOrWhiteSpace())
            {
                return false;
            }

            try
            {
                new CultureInfo(cultureCode); //TODO: Do not create a new object! How?
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}
