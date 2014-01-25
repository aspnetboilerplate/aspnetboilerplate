using System.Web;
using Abp.Dependency;
using Abp.Localization.Sources.Xml;

namespace Abp.Web.Localization
{
    /// <summary>
    /// Defines a localization source to get localization strings from XML files.
    /// </summary>
    public class AbpWebLocalizationSource : XmlLocalizationSource, ISingletonDependency
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