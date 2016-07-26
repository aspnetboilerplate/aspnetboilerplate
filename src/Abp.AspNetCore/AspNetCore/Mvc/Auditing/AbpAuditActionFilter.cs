using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Aspects;
using Abp.AspNetCore.Mvc.Extensions;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Abp.AspNetCore.Mvc.Auditing
{
    public class AbpAuditActionFilter : IAsyncActionFilter, ITransientDependency
    {
        /// <summary>
        /// Ignored types for serialization on audit logging.
        /// </summary>
        public static List<Type> IgnoredTypesForSerializationOnAuditLogging { get; private set; }

        public IAuditInfoProvider AuditInfoProvider { get; set; }

        public IAuditingStore AuditingStore { get; set; }

        public IAbpSession AbpSession { get; set; }

        public ILogger Logger { get; set; }
        
        private readonly IAuditingConfiguration _auditingConfiguration;
             
        static AbpAuditActionFilter()
        {
            IgnoredTypesForSerializationOnAuditLogging = new List<Type>();
        }

        public AbpAuditActionFilter(IAuditingConfiguration auditingConfiguration)
        {
            _auditingConfiguration = auditingConfiguration;

            AbpSession = NullAbpSession.Instance;
            AuditingStore = SimpleLogAuditingStore.Instance;
            AuditInfoProvider = NullAuditInfoProvider.Instance;
            Logger = NullLogger.Instance;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            using (AbpCrossCuttingConcerns.Applying(context.Controller, AbpCrossCuttingConcerns.Auditing))
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
                    var result = await next();
                    if (result.Exception != null && !result.ExceptionHandled)
                    {
                        auditInfo.Exception = result.Exception;
                    }
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
                    AuditInfoProvider?.Fill(auditInfo);
                    await AuditingStore.SaveAsync(auditInfo);
                }
            }
        }

        private AuditInfo CreateAuditInfo(ActionExecutingContext context)
        {
            var auditInfo = new AuditInfo
            {
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ServiceName = context.Controller?.GetType().ToString() ?? "",
                MethodName = context.ActionDescriptor.DisplayName,
                Parameters = ConvertArgumentsToJson(context.ActionArguments),
                ExecutionTime = Clock.Now
            };

            AuditInfoProvider.Fill(auditInfo);

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
                Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }
    }
}
