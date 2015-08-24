using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Timing;
using Castle.Core.Logging;
using Castle.DynamicProxy;

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
            if (!AuditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget, _configuration, AbpSession))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = CreateAuditInfo(invocation);

            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformAsyncAuditing(invocation, auditInfo);
            }
            else
            {
                PerformSyncAuditing(invocation, auditInfo);
            }
        }

        private AuditInfo CreateAuditInfo(IInvocation invocation)
        {
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

            return auditInfo;
        }

        private void PerformSyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
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
                AuditingStore.Save(auditInfo);
            }
        }
        private void PerformAsyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            invocation.Proceed();

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.WaitTaskAndAction(
                    (Task) invocation.ReturnValue,
                    async () =>
                    {
                        stopwatch.Stop();
                        auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                        await AuditingStore.SaveAsync(auditInfo);
                    });
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallReturnGenericTaskAfterAction(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () =>
                    {
                        stopwatch.Stop();
                        auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
                        await AuditingStore.SaveAsync(auditInfo);
                    },
                    () => { }); //TODO: Make an overload that not receives finally action!
            }
            
            //TODO: Handle exceptions!
            //try
            //{
            //}
            //catch (Exception ex)
            //{
            //    auditInfo.Exception = ex;
            //    throw;
            //}
            //finally
            //{
            //    stopwatch.Stop();
            //    auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            //    AuditingStore.Save(auditInfo);
            //}
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

                return JsonHelper.ConvertToJson(dictionary, true);
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not serialize arguments for method: " + invocation.MethodInvocationTarget.Name);
                Logger.Warn(ex.ToString(), ex);
                return "{}";
            }
        }
    }
}