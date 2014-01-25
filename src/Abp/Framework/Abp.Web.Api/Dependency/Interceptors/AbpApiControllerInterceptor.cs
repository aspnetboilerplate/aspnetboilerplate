using System.Web.Http;
using Abp.Dependency;
using Castle.DynamicProxy;

namespace Abp.WebApi.Dependency.Interceptors
{
    public class AbpApiControllerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            if (!invocation.MethodInvocationTarget.IsPublic)
            {
                invocation.Proceed();
                return;                
            }

            invocation.Proceed();
        }
    }
}
