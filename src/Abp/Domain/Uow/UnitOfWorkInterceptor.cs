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
        private readonly IUowManager _uowManager;

        public UnitOfWorkInterceptor(IIocResolver iocResolver, IUowManager uowManager)
        {
            _iocResolver = iocResolver;
            _uowManager = uowManager;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            if (_uowManager.Current != null)
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
            PerformUow(invocation, unitOfWorkAttr.IsTransactional != false);
        }

        private void PerformUow(IInvocation invocation, bool isTransactional)
        {
            if (!AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformSyncUow(invocation, isTransactional);
            }
            else
            {
                PerformAsyncUow(invocation, isTransactional);
            }
        }

        private void PerformSyncUow(IInvocation invocation, bool isTransactional)
        {
            using (var uow = _uowManager.StartNew(isTransactional))
            {
                invocation.Proceed();
                uow.Complete();
            }
        }

        private void PerformAsyncUow(IInvocation invocation, bool isTransactional)
        {
            var uow = _uowManager.StartNew(isTransactional);

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