using System.Web;
using Abp.Localization.Sources.XmlFiles;

namespace Abp.Web.Localization
{
    /// <summary>
    /// Defines a localization source to get localization strings from XML files.
    /// </summary>
    public class AbpWebLocalizationSource : XmlLocalizationSource
    {
        /// <summary>
        /// Constrictor.
        /// </summary>
        public AbpWebLocalizationSource()
            : base("Abp.Web", HttpContext.Current.Server.MapPath("Localization\\Modules\\Abp.Web"))
        {

        }
    }
}