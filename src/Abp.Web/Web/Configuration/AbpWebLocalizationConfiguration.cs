namespace Abp.Web.Configuration
{
    public class AbpWebLocalizationConfiguration : IAbpWebLocalizationConfiguration
    {
        /// <summary>
        /// Default: "Abp.Localization.CultureName".
        /// </summary>
        public string CookieName { get; set; }

        public AbpWebLocalizationConfiguration()
        {
            CookieName = "Abp.Localization.CultureName";
        }
    }
}