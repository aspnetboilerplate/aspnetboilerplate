using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;
using Castle.DynamicProxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        public IAbpSession AbpSession { get; set; }

        public ILogger Logger { get; set; }

        public IAuditingStore AuditingStore { get; set; }
        
        private readonly IAuditingConfiguration _configuration;
        private readonly IAuditInfoProvider _auditInfoProvider;

        public AuditingInterceptor(IAuditingConfiguration configuration, IAuditInfoProvider auditInfoProvider)
        {
            _configuration = configuration;
            _auditInfoProvider = auditInfoProvider;
            
            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            AuditingStore = SimpleLogAuditingStore.Instance;
        }

        public void Intercept(IInvocation invocation)
        {
            if (!_configuration.IsEnabled)
            {
                invocation.Proceed();
                return;
            }

            if (!ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = new AuditInfo
            {
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId,
                ServiceName = invocation.MethodInvocationTarget.DeclaringType != null
                              ? invocation.MethodInvocationTarget.DeclaringType.FullName
                              : "",
                MethodName = invocation.MethodInvocationTarget.Name,
                Parameters = ConvertArgumentsToJson(invocation),
                ExecutionTime = Clock.Now
            };

            _auditInfoProvider.Fill(auditInfo);

            var stopwatch = Stopwatch.StartNew();
            try
            {
                invocation.Proceed();
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
                AuditingStore.Save(auditInfo); //TODO: Call async when target method is async.
            }
        }

        private string ConvertArgumentsToJson(IInvocation invocation)
        {
            try
            {
                var parameters = invocation.MethodInvocationTarget.GetParameters();
                if (parameters.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();
                for (int i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var argument = invocation.Arguments[i];
                    dictionary[parameter.Name] = argument;
                }

                return JsonConvert.SerializeObject(
                    dictionary,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not serialize arguments for method: " + invocation.MethodInvocationTarget.Name);
                Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }

        private bool ShouldSaveAudit(MethodInfo methodInfo)
        {
            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof (AuditedAttribute)))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(DisableAuditingAttribute)))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.IsDefined(typeof(AuditedAttribute)))
                {
                    return true;
                }

                if (classType.IsDefined(typeof(DisableAuditingAttribute)))
                {
                    return false;
                }

                if (_configuration.Selectors.Any(selector => selector.Predicate(classType)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}