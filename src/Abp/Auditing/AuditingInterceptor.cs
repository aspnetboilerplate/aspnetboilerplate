using Castle.DynamicProxy;

namespace Abp.Auditing
{
    internal class AuditingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            invocation.Proceed();
        }
    }
}