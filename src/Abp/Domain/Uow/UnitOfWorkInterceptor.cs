using Abp.Dependency;
using Castle.DynamicProxy;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Abp.Domain.Uow
{
    /// <summary>
    /// This interceptor is used to manage database connection and transactions.
    /// </summary>
    internal class UnitOfWorkInterceptor : IInterceptor
    {
        private readonly IIocResolver _iocResolver;
        private readonly UnitOfWorkScope _unitOfWorkScope;

        public UnitOfWorkInterceptor(IIocResolver iocResolver, UnitOfWorkScope unitOfWorkScope)
        {
            _iocResolver = iocResolver;
            _unitOfWorkScope = unitOfWorkScope;
        }

        /// <summary>
        /// Intercepts a method.
        /// </summary>
        /// <param name="invocation">Method invocation arguments</param>
        public void Intercept(IInvocation invocation)
        {
            var unitOfWorkAttr = UnitOfWorkAttribute.GetUnitOfWorkAttributeOrDefault(invocation.MethodInvocationTarget);
            if (unitOfWorkAttr == null || unitOfWorkAttr.IsDisabled)
            {
                // No need to a uow
                invocation.Proceed();
                return;
            }

            // Perform the work.
            PerformUow(invocation, unitOfWorkAttr.IsTransactional != false);
        }

        private void PerformUow(IInvocation invocation, bool isTransactional)
        {
            var returnType = invocation.Method.ReturnType;

            if (IsAsyncMethod(invocation.Method) && typeof(Task).IsAssignableFrom(returnType))
            {
                invocation.ReturnValue = InterceptAsync((dynamic)invocation.ReturnValue, invocation, isTransactional);
            }
            else
            {
                // Not needed, all the work will be done by UnitOfWorkScope.
            }
        }

        private static async Task InterceptAsync(Task task, IInvocation invocation, bool isTransactional)
        {
            await task.ConfigureAwait(false);
        }

        private static async Task<T> InterceptAsync<T>(Task<T> task, IUnitOfWork unitOfWork, IInvocation invocation)
        {
            return await task.ConfigureAwait(false);
        }

        private static bool IsAsyncMethod(MethodInfo methodInfo)
        {
            // Methods returning Task (or derived) types are marked with AsyncStateMachineAttribute by the compiler.
            // Unless somebody fiddles with the resulting IL in a wierd way this will work on 99% cases.
            return methodInfo.GetCustomAttribute<AsyncStateMachineAttribute>() != null;
        }
    }
}