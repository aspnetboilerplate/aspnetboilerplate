using Castle.Core.Logging;
using Castle.DynamicProxy;

namespace Abp.WebApi.Dependency.Interceptors
{
    public class AbpApiControllerInterceptor : IInterceptor
    {
        public ILogger Logger { get; set; }

        public void Intercept(IInvocation invocation)
        {
            if (!invocation.MethodInvocationTarget.IsPublic)
            {
                invocation.Proceed();
                return;                
            }

            invocation.Proceed();
            //Logger.Debug(invocation.ReturnValue.ToString());
        }
    }
}
