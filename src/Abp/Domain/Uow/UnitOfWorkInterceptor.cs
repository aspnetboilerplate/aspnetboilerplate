using Abp.Threading;
using Castle.DynamicProxy;
using System.Threading.Tasks;

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
                //If you specify the open transaction, will the current uow affairs open
                if (!_unitOfWorkManager.Current.Options.IsTransactional.Value)
                {
                    var unitOfWorkAttr = UnitOfWorkAttribute.GetUnitOfWorkAttributeOrNull(invocation.MethodInvocationTarget);
                    if (unitOfWorkAttr != null && !unitOfWorkAttr.IsDisabled && unitOfWorkAttr.IsTransactional.HasValue && unitOfWorkAttr.IsTransactional.Value)
                    {
                        _unitOfWorkManager.Current.Options.IsTransactional = unitOfWorkAttr.IsTransactional;
                        _unitOfWorkManager.Current.Options.IsolationLevel = unitOfWorkAttr.IsolationLevel;
                        _unitOfWorkManager.Current.Options.Scope = unitOfWorkAttr.Scope;
                        _unitOfWorkManager.BeginTransactional(_unitOfWorkManager.Current.Options);
                    }
                }
                //Continue with current uow
                invocation.Proceed();
            }
            else
            {
                var unitOfWorkAttr = UnitOfWorkAttribute.GetUnitOfWorkAttributeOrNull(invocation.MethodInvocationTarget);
                if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
                {
                    //No need to a uow
                    invocation.Proceed();
                }
                else
                {
                    //No current uow, run a new one
                    PerformUow(invocation, unitOfWorkAttr.CreateOptions());
                }
            }
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
                invocation.Proceed();
                uow.Complete();
            }
        }

        private void PerformAsyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            var uow = _unitOfWorkManager.Begin(options);

            invocation.Proceed();

            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                    (Task)invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    exception => uow.Dispose()
                    );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithPostActionAndFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () => await uow.CompleteAsync(),
                    (exception) => uow.Dispose()
                    );
            }
        }
    }
}