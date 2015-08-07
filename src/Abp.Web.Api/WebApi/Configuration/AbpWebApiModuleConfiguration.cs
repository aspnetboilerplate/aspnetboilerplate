using System.Web.Http;

namespace Abp.WebApi.Configuration
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