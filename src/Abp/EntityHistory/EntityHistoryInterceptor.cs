using Castle.DynamicProxy;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.EntityHistory
{
    internal class EntityHistoryInterceptor : AbpInterceptorBase, ITransientDependency
    {
        public IEntityChangeSetReasonProvider ReasonProvider { get; set; }

        public EntityHistoryInterceptor()
        {
            ReasonProvider = NullEntityChangeSetReasonProvider.Instance;
        }

        public override void InterceptSynchronous(IInvocation invocation)
        {
            var methodInfo = invocation.MethodInvocationTarget;
            var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                                   ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                invocation.Proceed();
                return;
            }

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                invocation.Proceed();
            }
        }

        protected override async Task InternalInterceptAsynchronous(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            var methodInfo = invocation.MethodInvocationTarget;
            var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                                   ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task.ConfigureAwait(false);
                return;
            }

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                proceedInfo.Invoke();
                var task = (Task)invocation.ReturnValue;
                await task.ConfigureAwait(false);
            }
        }

        protected override async Task<TResult> InternalInterceptAsynchronous<TResult>(IInvocation invocation)
        {
            var proceedInfo = invocation.CaptureProceedInfo();

            var methodInfo = invocation.MethodInvocationTarget;
            var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                                   ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();

            if (useCaseAttribute?.Description == null)
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult.ConfigureAwait(false);
            }

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                proceedInfo.Invoke();
                var taskResult = (Task<TResult>)invocation.ReturnValue;
                return await taskResult.ConfigureAwait(false);
            }
        }
    }
}
