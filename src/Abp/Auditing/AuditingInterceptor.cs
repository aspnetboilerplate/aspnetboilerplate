using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Abp.Runtime.Session;
using Castle.DynamicProxy;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IAuditingStore _auditingStore;
        private readonly IAuditingConfiguration _configuration;

        public AuditingInterceptor(IAuditingStore auditingStore, IAuditingConfiguration configuration)
        {
            _auditingStore = auditingStore;
            _configuration = configuration;
            AbpSession = NullAbpSession.Instance;
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

            //TODO: Refactor!
            var auditInfo = new AuditInfo
                            {
                                TenantId = AbpSession.TenantId,
                                UserId = AbpSession.UserId,
                                ServiceName = invocation.MethodInvocationTarget.DeclaringType.FullName,
                                MethodName = invocation.MethodInvocationTarget.Name,
                                Parameters = invocation.Arguments.ToString(), //TODO: Convert to JSON?
                                ExecutionTime = DateTime.Now //TODO: UtcNow?
                            };

            //TODO: Fill web layer informations

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
                _auditingStore.Save(auditInfo);
            }
        }

        private bool ShouldSaveAudit(MethodInfo methodInfo)
        {
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