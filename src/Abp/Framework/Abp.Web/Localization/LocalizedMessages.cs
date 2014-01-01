using System;
using Abp.Localization;
using Abp.Localization.Sources;

namespace Abp.Web.Localization
{
    internal static class LocalizedMessages
    {
        public static string InternalServerError { get { return L("InternalServerError"); } }

        //private static ILocalizationSource _abpWebLocalizationSource;

        //static LocalizedMessages()
        //{
        //    _abpWebLocalizationSource = LocalizationHelper.GetSource("Abp.Web");
        //}

        private static string L(string name)
        {
            try
            {
                //TODO: Get Source first!
                //TODO: Create Abp.Web source!
                return LocalizationHelper.GetString("Abp.Web", name);
            }
            catch (Exception)
            {
                return name; //TODO: ?
            }
        }
    }
}
