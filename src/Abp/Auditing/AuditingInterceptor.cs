using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.Dependency;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : CastleAbpInterceptorAdapter<AuditingInterceptor>
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

        protected bool ShouldIntercept(IAbpMethodInvocation invocation)
        {
            if (AbpCrossCuttingConcerns.IsApplied(invocation.TargetObject, AbpCrossCuttingConcerns.Auditing))
            {
                return false;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.Method))
            {
                return false;
            }

            return true;
        }

        protected override void InterceptSync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                invocation.Proceed();
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetObject.GetType(), invocation.Method, invocation.Arguments);

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

        protected override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                await invocation.ProceedAsync();
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetObject.GetType(), invocation.Method, invocation.Arguments);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                await invocation.ProceedAsync();
            }
            catch (Exception ex)
            {
                auditInfo.Exception = ex;
                throw;
            }
            finally
            {
                var task = invocation.ReturnValue as Task;
                await SaveAuditInfoAsync(auditInfo, stopwatch, task);
            }
        }

        private async Task SaveAuditInfoAsync(AuditInfo auditInfo, Stopwatch stopwatch, Task task)
        {
            stopwatch.Stop();
            auditInfo.ExecutionDuration = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            FillTaskResult(task, auditInfo);

            await _auditingHelper.SaveAsync(auditInfo);
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