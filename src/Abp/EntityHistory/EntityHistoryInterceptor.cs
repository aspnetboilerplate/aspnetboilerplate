using Castle.DynamicProxy;
using System.Linq;

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
            var useCaseAttribute = invocation.MethodInvocationTarget.GetCustomAttributes(typeof(UseCaseAttribute), true).Cast<UseCaseAttribute>().FirstOrDefault();
            var reason = useCaseAttribute?.Description;

            using (ReasonProvider.Use(reason))
            {
                invocation.Proceed();
            }
        }
    }
}
