using System.Web.Http;

namespace Adorable.WebApi.Configuration
{
    internal class AbpWebApiModuleConfiguration : IAbpWebApiModuleConfiguration
    {
        public HttpConfiguration HttpConfiguration { get; set; }

        public AbpWebApiModuleConfiguration()
        {
            HttpConfiguration = GlobalConfiguration.Configuration;
        }
    }
}