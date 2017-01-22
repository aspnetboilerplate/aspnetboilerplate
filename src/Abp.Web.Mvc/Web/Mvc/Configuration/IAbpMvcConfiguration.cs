using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Configuration
{
    public interface IAbpMvcConfiguration
    {
        /// <summary>
        /// Default UnitOfWorkAttribute for all actions.
        /// </summary>
        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        /// <summary>
        /// Default WrapResultAttribute for all actions.
        /// </summary>
        WrapResultAttribute DefaultWrapResultAttribute { get; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsValidationEnabledForControllers { get; set; }

        /// <summary>
        /// Default: true.
        /// </summary>
        bool IsAutomaticAntiForgeryValidationEnabled { get; set; }

        /// <summary>
        /// Used to enable/disable auditing for MVC controllers.
        /// Default: true.
        /// </summary>
        bool IsAuditingEnabled { get; set; }

        /// <summary>
        /// Used to enable/disable auditing for child MVC actions.
        /// Default: false.
        /// </summary>
        bool IsAuditingEnabledForChildActions { get; set; }
    }
}
