using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;

namespace Abp.EntityHistory
{
    internal class EntityHistoryInterceptor : CastleAbpInterceptorAdapter<EntityHistoryInterceptor>
    {
        public IEntityChangeSetReasonProvider ReasonProvider { get; set; }

        public EntityHistoryInterceptor()
        {
            ReasonProvider = NullEntityChangeSetReasonProvider.Instance;
        }

        protected override void InterceptSync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                invocation.Proceed();
                return;
            }

            var useCaseAttribute = GetUseCaseAttribute(invocation);
            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                invocation.Proceed();
            }
        }

        protected override async Task InterceptAsync(IAbpMethodInvocation invocation)
        {
            if (!ShouldIntercept(invocation))
            {
                await invocation.ProceedAsync();
                return;
            }

            var useCaseAttribute = GetUseCaseAttribute(invocation);
            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                await invocation.ProceedAsync();
            }
        }

        private UseCaseAttribute GetUseCaseAttribute(IAbpMethodInvocation invocation)
        {
            var methodInfo = invocation.GetMethodInvocationTarget();
            var useCaseAttribute = methodInfo.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault()
                                   ?? methodInfo.DeclaringType.GetCustomAttributes(true).OfType<UseCaseAttribute>().FirstOrDefault();
            return useCaseAttribute;
        }

        protected bool ShouldIntercept(IAbpMethodInvocation invocation)
        {
            var useCaseAttribute = GetUseCaseAttribute(invocation);
            return useCaseAttribute?.Description != null;
        }
    }
}
