using System;
using System.Diagnostics;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Web.Mvc.Configuration;
using Abp.Web.Mvc.Extensions;

namespace Abp.Web.Mvc.Auditing
{
    public class AbpMvcAuditFilter : IActionFilter, ITransientDependency
    {
        private readonly IAbpMvcConfiguration _configuration;
        private readonly IAuditingHelper _auditingHelper;

        public AbpMvcAuditFilter(IAbpMvcConfiguration configuration, IAuditingHelper auditingHelper)
        {
            _configuration = configuration;
            _auditingHelper = auditingHelper;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ShouldSaveAudit(filterContext))
            {
                AbpAuditFilterData.Set(filterContext.HttpContext, null);
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(
                filterContext.ActionDescriptor.GetMethodInfoOrNull(),
                filterContext.ActionParameters
            );

            var actionStopwatch = Stopwatch.StartNew();

            AbpAuditFilterData.Set(
                filterContext.HttpContext,
                new AbpAuditFilterData(
                    actionStopwatch,
                    auditInfo
                )
            );
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var auditData = AbpAuditFilterData.GetOrNull(filterContext.HttpContext);
            if (auditData == null)
            {
                return;
            }

            auditData.Stopwatch.Stop();

            auditData.AuditInfo.ExecutionDuration = Convert.ToInt32(auditData.Stopwatch.Elapsed.TotalMilliseconds);
            auditData.AuditInfo.Exception = filterContext.Exception;

            _auditingHelper.Save(auditData.AuditInfo);
        }

        private bool ShouldSaveAudit(ActionExecutingContext filterContext)
        {
            var currentMethodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            if (currentMethodInfo == null)
            {
                return false;
            }

            if (_configuration == null)
            {
                return false;
            }

            if (!_configuration.IsAuditingEnabled)
            {
                return false;
            }

            if (filterContext.IsChildAction && !_configuration.IsAuditingEnabledForChildActions)
            {
                return false;
            }

            return _auditingHelper.ShouldSaveAudit(currentMethodInfo, true);
        }
    }
}
