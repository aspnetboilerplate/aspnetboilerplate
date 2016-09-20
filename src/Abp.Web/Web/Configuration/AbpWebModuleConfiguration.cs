using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Configuration
{
    public class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public IAbpAntiForgeryWebConfiguration AntiForgery { get; }
        public IAbpWebLocalizationConfiguration Localization { get; }

        public AbpWebModuleConfiguration(
            IAbpAntiForgeryWebConfiguration antiForgery, 
            IAbpWebLocalizationConfiguration localization)
        {
            AntiForgery = antiForgery;
            Localization = localization;
        }
    }
}