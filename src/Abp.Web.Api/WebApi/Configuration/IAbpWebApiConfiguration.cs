using System.Web.Http;
using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.WebApi.Configuration
{
    /// <summary>
    /// Used to configure ABP WebApi module.
    /// </summary>
    public interface IAbpWebApiConfiguration
    {
        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        WrapResultAttribute DefaultWrapResultAttribute { get; }

        WrapResultAttribute DefaultDynamicApiWrapResultAttribute { get; }

        /// <summary>
        /// Gets/sets <see cref="HttpConfiguration"/>.
        /// </summary>
        HttpConfiguration HttpConfiguration { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsValidationEnabledForControllers { get; set; }
    }
}