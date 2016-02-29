using System.Web.OData.Builder;

namespace Adorable.WebApi.OData.Configuration
{
    internal class AbpWebApiODataModuleConfiguration : IAbpWebApiODataModuleConfiguration
    {
        public ODataConventionModelBuilder ODataModelBuilder { get; private set; }

        public AbpWebApiODataModuleConfiguration()
        {
            ODataModelBuilder  = new ODataConventionModelBuilder();
        }
    }
}