using System.Web.Http;
using Abp.Web.Models;

namespace Abp.WebApi.Configuration
{
    /// <summary>
    /// Used to configure ABP WebApi module.
    /// </summary>
    public interface IAbpWebApiModuleConfiguration
    {
        WrapResultAttribute DefaultWrapResultAttribute { get; set; }

        WrapResultAttribute DefaultDynamicApiWrapResultAttribute { get; set; }

        /// <summary>
        /// Gets/sets <see cref="HttpConfiguration"/>.
        /// </summary>
        HttpConfiguration HttpConfiguration { get; set; }
    }
}