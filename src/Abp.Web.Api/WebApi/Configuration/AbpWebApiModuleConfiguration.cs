using System.Web.Http;
using Abp.Web.Models;

namespace Abp.WebApi.Configuration
{
    internal class AbpWebApiModuleConfiguration : IAbpWebApiModuleConfiguration
    {
        public WrapResultAttribute DefaultWrapResultAttribute { get; set; }

        public WrapResultAttribute DefaultDynamicApiWrapResultAttribute { get; set; }

        public HttpConfiguration HttpConfiguration { get; set; }

        public AbpWebApiModuleConfiguration()
        {
            HttpConfiguration = GlobalConfiguration.Configuration;
            DefaultWrapResultAttribute = new WrapResultAttribute(false);
            DefaultDynamicApiWrapResultAttribute = new WrapResultAttribute();
        }
    }
}