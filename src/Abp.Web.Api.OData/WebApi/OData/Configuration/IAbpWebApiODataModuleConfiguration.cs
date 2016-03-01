using System.Web.OData.Builder;

namespace Adorable.WebApi.OData.Configuration
{
    /// <summary>
    /// Used to configure Adorable.Web.Api.OData module.
    /// </summary>
    public interface IAbpWebApiODataModuleConfiguration
    {
        /// <summary>
        /// Gets ODataConventionModelBuilder.
        /// </summary>
        ODataConventionModelBuilder ODataModelBuilder { get; }
    }
}