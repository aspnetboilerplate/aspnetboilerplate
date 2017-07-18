using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Configuration
{
    public interface IAbpWebModuleConfiguration
    {
        IAbpAntiForgeryWebConfiguration AntiForgery { get; }

        IAbpWebLocalizationConfiguration Localization { get; }
    }
}