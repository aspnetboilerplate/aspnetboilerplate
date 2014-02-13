using Abp.Application.Authorization;
using Castle.DynamicProxy;

namespace Abp.Web.Mvc.Dependency.Interceptors
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpControllerInterceptor : IInterceptor
    { 
        public void Intercept(IInvocation invocation)
        {
            if (!invocation.MethodInvocationTarget.IsPublic)
            {
                invocation.Proceed();
                return;                
            }

            AuthorizationInterceptionHelper.Authorize<AbpAuthorizeAttribute>(invocation);
            invocation.Proceed();
        }
    }
}
