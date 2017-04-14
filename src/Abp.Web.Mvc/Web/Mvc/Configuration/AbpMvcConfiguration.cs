using Abp.Domain.Uow;
using Abp.Web.Models;

namespace Abp.Web.Mvc.Configuration
{
    public class AbpMvcConfiguration : IAbpMvcConfiguration
    {
        /// <summary>
        /// 所有操作的默认UnitOfWorkAttribute。
        /// </summary>
        public UnitOfWorkAttribute DefaultUnitOfWorkAttribute { get; }
        /// <summary>
        /// 所有操作的默认WrapResultAttribute。
        /// </summary>
        public WrapResultAttribute DefaultWrapResultAttribute { get; }
        /// <summary>
        /// 默认值：true。
        /// </summary>
        public bool IsValidationEnabledForControllers { get; set; }
        /// <summary>
        /// 默认值：true。
        /// </summary>
        public bool IsAutomaticAntiForgeryValidationEnabled { get; set; }
        /// <summary>
        /// 用于启用/禁用MVC控制器的审核。
        /// 默认值：true。
        /// </summary>
        public bool IsAuditingEnabled { get; set; }
        /// <summary>
        /// 用于启用/禁用MVC子操作的审核。
        /// 默认值：false。
        /// </summary>
        public bool IsAuditingEnabledForChildActions { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        public AbpMvcConfiguration()
        {
            DefaultUnitOfWorkAttribute = new UnitOfWorkAttribute();
            DefaultWrapResultAttribute = new WrapResultAttribute();
            IsValidationEnabledForControllers = true;
            IsAutomaticAntiForgeryValidationEnabled = true;
            IsAuditingEnabled = true;
        }
    }
}