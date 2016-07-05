using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Abp.Aspects;
using Abp.Auditing;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.WebApi.Validation;
using Castle.Core.Logging;

namespace Abp.WebApi.Auditing
{
    public class AbpAuditFilter : IActionFilter, ITransientDependency
    {
        /// <summary>
        /// Ignored types for serialization on audit logging.
        /// </summary>
        public static List<Type> IgnoredTypesForSerializationOnAuditLogging { get; private set; }

        public bool AllowMultiple => false;

        public IAuditInfoProvider AuditInfoProvider { get; set; }

        public IAuditingStore AuditingStore { get; set; }

        public IAbpSession AbpSession { get; set; }

        public ILogger Logger { get; set; }

        private readonly IAuditingConfiguration _auditingConfiguration;

        static AbpAuditFilter()
        {
            IgnoredTypesForSerializationOnAuditLogging = new List<Type>();
        }

        public AbpAuditFilter(IAuditingConfiguration auditingConfiguration)
        {
            _auditingConfiguration = auditingConfiguration;

            AbpSession = NullAbpSession.Instance;
            AuditingStore = SimpleLogAuditingStore.Instance;
            AuditInfoProvider = NullAuditInfoProvider.Instance;
            Logger = NullLogger.Instance;
        }

        public async Task<HttpResponseMessage> ExecuteActionFilterAsync(HttpActionContext actionContext, CancellationToken cancellationToken, Func<Task<HttpResponseMessage>> continuation)
        {
            if (actionContext.ActionDescriptor.GetMethodInfoOrNull() == null ||
                !ShouldSaveAudit(actionContext))
            {
                return await continuation();
            }

            using (AbpCrossCuttingConcerns.Applying(actionContext.ControllerContext.Controller, AbpCrossCuttingConcerns.Auditing))
            {
                var auditInfo = CreateAuditInfo(actionContext);
                var stopwatch = Stopwatch.StartNew();

                try
                {
                    return await continuation();
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

        private AuditInfo CreateAuditInfo(HttpActionContext context)
        {
            var auditInfo = new AuditInfo
            {
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ServiceName = context.ControllerContext.Controller?.GetType().ToString() ?? "",
                MethodName = context.ActionDescriptor.ActionName,
                Parameters = ConvertArgumentsToJson(context.ActionArguments),
                ExecutionTime = Clock.Now
            };

            AuditInfoProvider.Fill(auditInfo);

            return auditInfo;
        }

        private bool ShouldSaveAudit(HttpActionContext filterContext)
        {
            if (!_auditingConfiguration.IsEnabled)
            {
                return false;
            }

            return AuditingHelper.ShouldSaveAudit(
                filterContext.ActionDescriptor.GetMethodInfoOrNull(),
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