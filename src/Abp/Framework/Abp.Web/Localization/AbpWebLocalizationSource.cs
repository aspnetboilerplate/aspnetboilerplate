using System.Web;
using Abp.Localization.Sources.XmlFiles;

namespace Abp.Web.Localization
{
    public class AbpWebLocalizationSource : XmlLocalizationSource
    {
        public AbpWebLocalizationSource()
            : base("Abp.Web", HttpContext.Current.Server.MapPath("Localization\\Modules\\Abp.Web"))
        {

        }
    }
}