using System;
using Abp.Localization;

namespace Abp.Web.Localization
{
    public static class AbpWebMessages
    {
        public static string InternalServerError { get { return L("InternalServerError"); } }

        private static string L(string name)
        {
            try
            {
                return LocalizationHelper.GetString(name);
            }
            catch (Exception)
            {
                return name; //TODO: ?
            }
        }
    }
}
