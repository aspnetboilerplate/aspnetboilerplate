using System;
using System.Diagnostics;
using Abp.Runtime.Session;
using Castle.DynamicProxy;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IAuditingStore _auditingStore;

        public AuditingInterceptor(IAuditingStore auditingStore)
        {
            _auditingStore = auditingStore;
            AbpSession = NullAbpSession.Instance;
        }

        public void Intercept(IInvocation invocation)
        {
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
    }
}