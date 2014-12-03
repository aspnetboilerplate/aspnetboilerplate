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

        public UnitOfWorkInterceptor(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            if (UnitOfWorkScope.Current != null)
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
            var unitOfWork = _iocResolver.Resolve<IUnitOfWork>();
            
            UnitOfWorkScope.Current = unitOfWork;
            UnitOfWorkScope.Current.Initialize(isTransactional);
            UnitOfWorkScope.Current.Begin();

            if (!AsyncHelper.IsAsyncMethod(invocation.Method))
            {
                PerformSyncUow(invocation);
            }
            else
            {
                PerformAsyncUow(invocation);
            }
        }

        private void PerformSyncUow(IInvocation invocation)
        {
            try
            {
                try
                {
                    invocation.Proceed();

                    UnitOfWorkScope.Current.End();
                }
                catch
                {
                    try { UnitOfWorkScope.Current.Cancel(); }
                    catch { } //Hide exceptions on cancelling
                    throw;
                }
            }
            finally
            {
                UnitOfWorkScope.Current = null;
            }
        }

        private void PerformAsyncUow(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();

                Task result;
                if (invocation.Method.ReturnType == typeof(Task))
                {
                    result = AsyncHelper.ReturnTaskAfterAction(
                        (Task)invocation.ReturnValue,
                        async () => await UnitOfWorkScope.Current.EndAsync()
                        );
                }
                else
                {
                    result = (Task)AsyncHelper.CallReturnAfterAction(
                        invocation.Method.ReturnType.GenericTypeArguments[0],
                        invocation.ReturnValue,
                        async () => await UnitOfWorkScope.Current.EndAsync()
                        );
                }

                result.ContinueWith(task =>
                                    {
                                        _iocResolver.Release(UnitOfWorkScope.Current);
                                        UnitOfWorkScope.Current = null;
                                    });

                invocation.ReturnValue = result;
            }
            catch
            {
                try { UnitOfWorkScope.Current.Cancel(); }
                catch { } //Hide exceptions on cancelling
                throw;
            }
        }
    }
}