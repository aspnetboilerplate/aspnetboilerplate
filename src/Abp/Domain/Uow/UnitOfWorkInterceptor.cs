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
            using (var unitOfWork = _iocResolver.ResolveAsDisposable<IUnitOfWork>())
            {
                UnitOfWorkScope.Current = unitOfWork.Object;
                UnitOfWorkScope.Current.Initialize(isTransactional);
                UnitOfWorkScope.Current.Begin();

                try
                {
                    try
                    {
                        invocation.Proceed();

                        if (AsyncHelper.IsAsyncMethod(invocation.Method))
                        {
                            if (invocation.Method.ReturnType == typeof(Task))
                            {
                                invocation.ReturnValue = AsyncHelper.ReturnAfterAction((Task)invocation.ReturnValue, () => UnitOfWorkScope.Current.EndAsync());
                            }
                            else
                            {
                                invocation.ReturnValue = AsyncHelper.CallReturnAfterAction(
                                    invocation.Method.ReturnType.GenericTypeArguments[0],
                                    invocation.ReturnValue,
                                    () => UnitOfWorkScope.Current.EndAsync()
                                    );
                            }
                        }
                        else
                        {
                            UnitOfWorkScope.Current.End();
                        }
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
        }
    }
}