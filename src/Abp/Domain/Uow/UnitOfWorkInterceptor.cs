using System.Reflection;
using System.Threading.Tasks;
using Abp.Dependency;
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

                        if (IsAsyncMethod(invocation.Method))
                        {
                            if (invocation.Method.ReturnType == typeof(Task))
                            {
                                invocation.ReturnValue = VoidCall((Task)invocation.ReturnValue, UnitOfWorkScope.Current);
                            }
                            else
                            {
                                var genericArgType = invocation.Method.ReturnType.GenericTypeArguments[0];
                                invocation.ReturnValue =
                                    GetType()
                                    .GetMethod("NotVoidCall", BindingFlags.Public | BindingFlags.Instance)
                                    .MakeGenericMethod(genericArgType)
                                    .Invoke(this, new[] { invocation.ReturnValue, UnitOfWorkScope.Current });
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

        private static bool IsAsyncMethod(MethodInfo method)
        {
            return (
                method.ReturnType == typeof(Task) ||
                (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
                );
        }

        public async Task VoidCall(Task actualReturnValue, IUnitOfWork unitOfWork)
        {
            await actualReturnValue;
            await unitOfWork.EndAsync();
        }

        public async Task<T> NotVoidCall<T>(Task<T> actualReturnValue, IUnitOfWork unitOfWork)
        {
            var response = await actualReturnValue;
            await unitOfWork.EndAsync();
            return response;
        }
    }
}