using Castle.DynamicProxy;

namespace Abp.Web.Dependency.Interceptors
{
    public class AbpApiControllerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if(!invocation.MethodInvocationTarget.IsPublic)
            {
                invocation.Proceed();
                return;
            }

            invocation.Proceed();
        }
    }
}
