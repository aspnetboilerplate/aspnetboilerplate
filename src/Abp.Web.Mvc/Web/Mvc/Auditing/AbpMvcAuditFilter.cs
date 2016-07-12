using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.Web.Mvc.Extensions;
using Castle.Core.Logging;

namespace Abp.Web.Mvc.Auditing
{
    public class AbpMvcAuditFilter : IActionFilter, ITransientDependency
    {
        /// <summary>
        /// Ignored types for serialization on audit logging.
        /// </summary>
        public static List<Type> IgnoredTypesForSerializationOnAuditLogging { get; private set; }

        public IAbpSession AbpSession { get; set; }
        public IAuditInfoProvider AuditInfoProvider { get; set; }
        public IAuditingStore AuditingStore { get; set; }
        public ILogger Logger { get; set; }

        private readonly IAuditingConfiguration _auditingConfiguration;

        static AbpMvcAuditFilter()
        {
            IgnoredTypesForSerializationOnAuditLogging = new List<Type>
            {
                typeof (HttpPostedFileBase),
                typeof (IEnumerable<HttpPostedFileBase>)
            };
        }

        public AbpMvcAuditFilter(IAuditingConfiguration auditingConfiguration)
        {
            _auditingConfiguration = auditingConfiguration;

            AbpSession = NullAbpSession.Instance;
            AuditingStore = SimpleLogAuditingStore.Instance;
            AuditInfoProvider = NullAuditInfoProvider.Instance;
            Logger = NullLogger.Instance;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!ShouldSaveAudit(filterContext))
            {
                AbpAuditFilterData.Set(filterContext.HttpContext, null);
                return;
            }

            var currentMethodInfo = filterContext.ActionDescriptor.GetMethodInfoOrNull();
            var actionStopwatch = Stopwatch.StartNew();
            var auditInfo = new AuditInfo
            {
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ServiceName = currentMethodInfo.DeclaringType != null
                                ? currentMethodInfo.DeclaringType.FullName
                                : filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                MethodName = currentMethodInfo.Name,
                Parameters = ConvertArgumentsToJson(filterContext),
                ExecutionTime = Clock.Now
            };

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

            AuditInfoProvider?.Fill(auditData.AuditInfo);
            AuditingStore.Save(auditData.AuditInfo);
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

            return AuditingHelper.ShouldSaveAudit(
                currentMethodInfo,
                _auditingConfiguration,
                AbpSession,
                true
                );
        }

        private string ConvertArgumentsToJson(ActionExecutingContext filterContext)
        {
            try
            {
                if (filterContext.ActionParameters.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in filterContext.ActionParameters)
                {
                    if (argument.Value != null && IgnoredTypesForSerializationOnAuditLogging.Any(t => t.IsInstanceOfType(argument.Value)))
                    {
                        dictionary[argument.Key] = null;
                    }
                    else
                    {
                        dictionary[argument.Key] = argument.Value;
                    }
                }

                return AuditingHelper.Serialize(dictionary);
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not serialize arguments for method: " + filterContext.Controller.GetType().FullName + "." + filterContext.ActionDescriptor.ActionName);
                Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }
    }
}
