using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Filters
{
    //TODO: MOVE MVC AUDIT CONFIGURATION TO RELATED MODULES, REMOVE FROM ABP PACKAGE

    public class AbpAuditActionFilter : IAsyncActionFilter
    {
        private readonly IAuditingConfiguration _auditingConfiguration;

        private readonly IAuditInfoProvider _auditInfoProvider;

        private readonly IAuditingStore _auditingStore;

        private readonly IAbpSession _abpSession;

        public IAbpSession AbpSession { get; set; }

        /// <summary>
        /// Ignored types for serialization on audit logging.
        /// </summary>
        protected static List<Type> IgnoredTypesForSerializationOnAuditLogging { get; private set; }

        private readonly ILogger _logger;

        static AbpAuditActionFilter()
        {
            IgnoredTypesForSerializationOnAuditLogging = new List<Type>();
        }

        public AbpAuditActionFilter(
            IAuditingConfiguration auditingConfiguration, 
            IAuditInfoProvider auditInfoProvider, 
            IAuditingStore auditingStore, 
            IAbpSession abpSession,
            ILogger logger)
        {
            _auditingConfiguration = auditingConfiguration;
            _auditInfoProvider = auditInfoProvider;
            _auditingStore = auditingStore;
            _abpSession = abpSession;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!ShouldSaveAudit(context))
            {
                await next();
                return;
            }

            var auditInfo = CreateAuditInfo(context);
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await next();
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                stopwatch.Stop();
                auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                _auditInfoProvider?.Fill(auditInfo);
                await _auditingStore.SaveAsync(auditInfo);
            }
        }

        private AuditInfo CreateAuditInfo(ActionExecutingContext context)
        {
            var auditInfo = new AuditInfo
            {
                TenantId = _abpSession.TenantId,
                UserId = _abpSession.UserId,
                ImpersonatorUserId = _abpSession.ImpersonatorUserId,
                ImpersonatorTenantId = _abpSession.ImpersonatorTenantId,
                ServiceName = context.Controller?.GetType().ToString() ?? "",
                MethodName = context.ActionDescriptor.DisplayName,
                Parameters = ConvertArgumentsToJson(context.ActionArguments),
                ExecutionTime = Clock.Now
            };

            _auditInfoProvider.Fill(auditInfo);

            return auditInfo;
        }

        private bool ShouldSaveAudit(ActionExecutingContext filterContext)
        {
            if (!_auditingConfiguration.IsEnabled || !_auditingConfiguration.MvcControllers.IsEnabled)
            {
                return false;
            }

            return AuditingHelper.ShouldSaveAudit(
                filterContext.ActionDescriptor.GetMethodInfo(),
                _auditingConfiguration,
                AbpSession,
                true
                );
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
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
                _logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }
    }
}
