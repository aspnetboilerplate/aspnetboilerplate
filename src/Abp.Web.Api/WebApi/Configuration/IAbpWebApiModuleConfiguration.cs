using System.Web.Http;

namespace Abp.WebApi.Configuration
{
    /// <summary>
    /// Used to configure ABP WebApi module.
    /// </summary>
    public interface IAbpWebApiModuleConfiguration
    {
        /// <summary>
        /// Gets/sets <see cref="HttpConfiguration"/>.
        /// </summary>
        HttpConfiguration HttpConfiguration { get; set; }
    }
}