using System.Globalization;

namespace Abp.Utils.Helpers
{
    internal static class GlobalizationHelper
    {
        public static bool IsValidCultureCode(string cultureCode)
        {
            try
            {
                CultureInfo.GetCultureInfo(cultureCode);
                return true;
            }
            catch (CultureNotFoundException)
            {
                return false;
            }
        }
    }
}
