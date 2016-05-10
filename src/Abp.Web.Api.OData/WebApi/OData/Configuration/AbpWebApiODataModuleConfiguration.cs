using System.Web.OData.Builder;

namespace Abp.WebApi.OData.Configuration
{
    internal class AbpWebApiODataModuleConfiguration : IAbpWebApiODataModuleConfiguration
    {
        public AbpWebApiODataModuleConfiguration()
        {
            ODataModelBuilder = new ODataConventionModelBuilder();
        }

        public ODataConventionModelBuilder ODataModelBuilder { get; }
    }
}