using Abp.Localization.Sources.Xml;

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
            : base("AbpWeb", "Localization\\AbpWeb")
        {

        }
    }
}