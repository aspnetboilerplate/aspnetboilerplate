using Castle.DynamicProxy;
using System.Linq;
using System.Reflection;

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
            var useCaseAttribute = methodInfo.GetCustomAttributes<UseCaseAttribute>(true).FirstOrDefault()
                  ?? methodInfo.DeclaringType.GetCustomAttributes<UseCaseAttribute>(true).First();

            using (ReasonProvider.Use(useCaseAttribute.Description))
            {
                invocation.Proceed();
            }
        }
    }
}
