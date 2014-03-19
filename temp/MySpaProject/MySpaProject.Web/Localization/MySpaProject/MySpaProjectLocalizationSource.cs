using System.Web;
using Abp.Dependency;
using Abp.Localization.Sources.Xml;

namespace MySpaProject.Web.Localization.MySpaProject
{
    public class MySpaProjectLocalizationSource : XmlLocalizationSource, ISingletonDependency //TODO: Remove ISingletonDependency after Abp 0.1.5.0
    {
        public MySpaProjectLocalizationSource()
            : base("MySpaProject", HttpContext.Current.Server.MapPath("/Localization/MySpaProject"))
        {
        }
    }
}