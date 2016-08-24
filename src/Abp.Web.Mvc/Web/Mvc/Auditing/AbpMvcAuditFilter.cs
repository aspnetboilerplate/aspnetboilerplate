using System;
using System.Diagnostics;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Web.Mvc.Extensions;

namespace Abp.Web.Mvc.Auditing
{
    public class AbpMvcAuditFilter : IActionFilter, ITransientDependency
    {
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IAuditingHelper _auditingHelper;

        public AbpMvcAuditFilter(IAuditingConfiguration auditingConfiguration, IAuditingHelper auditingHelper)
        {
            _auditingConfiguration = auditingConfiguration;
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

            if (_auditingConfiguration == null)
            {
                return false;
            }

            if (!_auditingConfiguration.MvcControllers.IsEnabled)
            {
                return false;
            }

            if (filterContext.IsChildAction && !_auditingConfiguration.MvcControllers.IsEnabledForChildActions)
            {
                return false;
            }

            return _auditingHelper.ShouldSaveAudit(currentMethodInfo,true);
        }
    }
}
