using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.Threading;
using Castle.DynamicProxy;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        private readonly IAuditingHelper _auditingHelper;
        private readonly IAuditingConfiguration _auditingConfiguration;
        private readonly IAuditSerializer _auditSerializer;

        public AuditingInterceptor(
            IAuditingHelper auditingHelper, 
            IAuditingConfiguration auditingConfiguration, 
            IAuditSerializer auditSerializer)
        {
            _auditingHelper = auditingHelper;
            _auditingConfiguration = auditingConfiguration;
            _auditSerializer = auditSerializer;
        }

        public void Intercept(IInvocation invocation)
        {
            if (AbpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, AbpCrossCuttingConcerns.Auditing))
            {
                invocation.Proceed();
                return;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            if (invocation.Method.IsAsync())
            {
                PerformAsyncAuditing(invocation, auditInfo);
            }
            else
            {
                PerformSyncAuditing(invocation, auditInfo);
            }
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

                if (_auditingConfiguration.SaveReturnValues && invocation.ReturnValue != null)
                {
                    auditInfo.ReturnValue = _auditSerializer.Serialize(invocation.ReturnValue);
                }

                _auditingHelper.Save(auditInfo);
            }
        }

        private void PerformAsyncAuditing(IInvocation invocation, AuditInfo auditInfo)
        {
            var stopwatch = Stopwatch.StartNew();

            invocation.Proceed();

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithFinally(
                    (Task)invocation.ReturnValue,
                    exception => SaveAuditInfo(auditInfo, stopwatch, exception, null)
                    );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    (exception, task) => SaveAuditInfo(auditInfo, stopwatch, exception, task)
                    );
            }
        }

        private void SaveAuditInfo(AuditInfo auditInfo, Stopwatch stopwatch, Exception exception, Task task)
        {
            stopwatch.Stop();
            auditInfo.Exception = exception;
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            FillTaskResult(task, auditInfo);

            _auditingHelper.Save(auditInfo);
        }

        private void FillTaskResult(Task task, AuditInfo auditInfo)
        {
            if (_auditingConfiguration.SaveReturnValues && task != null && task.Status == TaskStatus.RanToCompletion)
            {
                auditInfo.ReturnValue = _auditSerializer.Serialize(task.GetType().GetTypeInfo()
                    .GetProperty("Result")
                    ?.GetValue(task, null));
            }
        }
    }
}