using System.Web.Http;

namespace Abp.WebApi.Configuration
{
    internal class AbpWebApiModuleConfiguration : IAbpWebApiModuleConfiguration
    {
        public AbpWebApiModuleConfiguration()
        {
            HttpConfiguration = GlobalConfiguration.Configuration;
        }

        public HttpConfiguration HttpConfiguration { get; set; }
    }
}