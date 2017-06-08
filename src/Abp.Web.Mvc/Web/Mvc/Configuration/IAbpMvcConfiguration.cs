using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Configuration
{
    public interface IAbpMvcConfiguration
    {
        /// <summary>
        /// 所有操作的默认UnitOfWorkAttribute。
        /// </summary>
        UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }

        /// <summary>
        /// 所有操作的默认WrapResultAttribute。
        /// </summary>
        WrapResultAttribute DefaultWrapResultAttribute { get; }

        /// <summary>
        /// 默认值：true。
        /// </summary>
        bool IsValidationEnabledForControllers { get; set; }

        /// <summary>
        /// 默认值：true。
        /// </summary>
        bool IsAutomaticAntiForgeryValidationEnabled { get; set; }

        /// <summary>
        /// 用于启用/禁用MVC控制器的审核。
        /// 默认值：true。
        /// </summary>
        bool IsAuditingEnabled { get; set; }

        /// <summary>
        /// 用于启用/禁用MVC子操作的审核。
        /// 默认值：false。
        /// </summary>
        bool IsAuditingEnabledForChildActions { get; set; }
    }
}
