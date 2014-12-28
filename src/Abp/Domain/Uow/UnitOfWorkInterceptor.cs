using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Reflection;
using Castle.DynamicProxy;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    internal class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly IIocResolver _iocResolver;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public UnitOfWorkInterceptor(IIocResolver iocResolver, IUnitOfWorkManager unitOfWorkManager)
        {
            _iocResolver = iocResolver;
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
            if (!AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformSyncUow(invocation, options);
            }
            else
            {
                PerformAsyncUow(invocation, options);
            }
        }

        private void PerformSyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            using (var uow = _unitOfWorkManager.StartNew(options))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        private void PerformAsyncUow(IInvocation invocation, UnitOfWorkOptions options)
        {
            var uow = _unitOfWorkManager.StartNew(options);

            invocation.Proceed();

            Task result;
            if (invocation.Method.ReturnType == typeof (Task))
            {
                result = AsyncHelper.ReturnTaskAfterAction(
                    (Task) invocation.ReturnValue,
                    async () =>
                          {
                              await uow.CompleteAsync();
                              uow.Dispose();
                          }
                    );
            }
            else
            {
                result = (Task) AsyncHelper.CallReturnAfterAction(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    invocation.ReturnValue,
                    async () =>
                          {
                              await uow.CompleteAsync();
                              uow.Dispose();
                              _iocResolver.Release(uow);
                          }
                    );
            }

            invocation.ReturnValue = result;
        }
    }
}