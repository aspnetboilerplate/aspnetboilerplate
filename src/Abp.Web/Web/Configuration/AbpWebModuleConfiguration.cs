using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Configuration
{
    public class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public IAbpAntiForgeryWebConfiguration AntiForgery { get; }

        public AbpWebModuleConfiguration(IAbpAntiForgeryWebConfiguration antiForgery)
        {
            AntiForgery = antiForgery;
        }
    }
}