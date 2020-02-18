using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Abp.Aspects;
using Abp.Dependency;
using Castle.DynamicProxy;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : AbpInterceptorBase, ITransientDependency
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

        public override void InterceptSynchronous(IInvocation invocation)
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

        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (AbpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, AbpCrossCuttingConcerns.Auditing))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
                return;
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            var stopwatch = Stopwatch.StartNew();

            try
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task;
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

                await _auditingHelper.SaveAsync(auditInfo).ConfigureAwait(false);
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            if (AbpCrossCuttingConcerns.IsApplied(invocation.InvocationTarget, AbpCrossCuttingConcerns.Auditing))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult.ConfigureAwait(false);
            }

            if (!_auditingHelper.ShouldSaveAudit(invocation.MethodInvocationTarget))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult.ConfigureAwait(false);
            }

            var auditInfo = _auditingHelper.CreateAuditInfo(invocation.TargetType, invocation.MethodInvocationTarget, invocation.Arguments);

            var stopwatch = Stopwatch.StartNew();
            TResult result;

            try
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                result = await taskResult.ConfigureAwait(false);

                if (_auditingConfiguration.SaveReturnValues && result != null)
                {
                    auditInfo.ReturnValue = _auditSerializer.Serialize(result);
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

                await _auditingHelper.SaveAsync(auditInfo).ConfigureAwait(false);
            }

            return result;
        }
    }
}