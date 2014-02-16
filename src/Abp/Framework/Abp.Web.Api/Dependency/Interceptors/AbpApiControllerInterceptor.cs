using Abp.Application.Authorization;
using Castle.DynamicProxy;

namespace Abp.WebApi.Dependency.Interceptors
{
    /// <summary>
    /// 
    /// </summary>
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
