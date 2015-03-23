using System.Threading.Tasks;
using Abp.Threading;
using Castle.DynamicProxy;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    internal class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkInterceptor(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            if (_unitOfWorkManager.Current != null)
            {
                //Continue with current uow
                invocation.Proceed();
                return;
            }

            var unitOfWorkAttr = UnitOfWorkAttribute.GetUnitOfWorkAttributeOrNull(invocation.MethodInvocationTarget);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                //No need to a uow
                invocation.Proceed();
                return;
            }

            //No current uow, run a new one
            PerformUow(invocation, unitOfWorkAttr.CreateOptions());
        }

        private void PerformUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            if (AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformAsyncUow(invocation, options);
            }
            else
            {
                PerformSyncUow(invocation, options);
            }
        }

        private void PerformSyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            using (var uow = _unitOfWorkManager.Begin(options))
            {
                try
                {
                    invocation.Proceed();
                    uow.Complete();
                }
                catch (System.Exception ex)
                {
                    // Save to unit of work so the error will propagate to the failed handler correctly
                    UnitOfWorkBase unitOfWorkBase = _unitOfWorkManager.Current as UnitOfWorkBase;
                    if (unitOfWorkBase != null)
                    {
                        unitOfWorkBase.SetException(ex);
                    }
                    // rethrow
                    throw;
                }
            }
        }

        private void PerformAsyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            var uow = _unitOfWorkManager.Begin(options);

            try
            {
                invocation.Proceed();
            }
            catch (System.Exception ex)
            {
                // Save to unit of work so the error will propagate to the failed handler correctly
                UnitOfWorkBase unitOfWorkBase = _unitOfWorkManager.Current as UnitOfWorkBase;
                if (unitOfWorkBase != null)
                {
                    unitOfWorkBase.SetException(ex);
                }
                // rethrow
                throw;
            }

            if (invocation.Method.ReturnType == typeof (Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.WaitTaskAndActionWithFinally(
                    (Task) invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    uow.Dispose
                    );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallReturnGenericTaskAfterAction(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    uow.Dispose
                    );
            }
        }
    }
}