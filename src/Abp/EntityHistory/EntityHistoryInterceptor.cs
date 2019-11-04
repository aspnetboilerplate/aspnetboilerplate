using Castle.DynamicProxy;
using System.Linq;
using System.Threading.Tasks;
using Abp.Threading;

namespace Abp.EntityHistory
{
    internal class EntityHistoryInterceptor : IInterceptor
    {
        public IEntityChangeSetReasonProvider ReasonProvider { get; set; }

        public EntityHistoryInterceptor()
        {
            ReasonProvider = NullEntityChangeSetReasonProvider.Instance;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                  ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                invocation.Proceed();
                return;
            }

            if (invocation.Method.IsAsync())
            {
                PerformAsyncUow(invocation, useCaseAttribute);
            }
            else
            {
                PerformSyncUow(invocation, useCaseAttribute);
            }
        }

        private void PerformSyncUow(IInvocation invocation, UseCaseAttribute useCaseAttribute)
        {
            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                invocation.Proceed();
            }
        }

        private void PerformAsyncUow(IInvocation invocation, UseCaseAttribute useCaseAttribute)
        {
            if (invocation.Method.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = InternalAsyncHelper.AwaitTaskWithUsingActionAndFinally(
                    () => ReasonProvider.Use(useCaseAttribute.Description),
                    () =>
                    {
                        invocation.Proceed();
                        return (Task) invocation.ReturnValue;
                    },
                    exception => { }
                );
            }
            else //Task<TResult>
            {
                invocation.ReturnValue = InternalAsyncHelper.CallAwaitTaskWithUsingActionAndFinallyAndGetResult(
                    invocation.Method.ReturnType.GenericTypeArguments[0],
                    () => ReasonProvider.Use(useCaseAttribute.Description),
                    () =>
                    {
                        invocation.Proceed();
                        return invocation.ReturnValue;
                    },
                    exception => { }
                );
            }
        }
    }
}
