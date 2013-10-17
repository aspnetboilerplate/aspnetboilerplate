using Abp.Validation;
using Castle.DynamicProxy;

namespace Abp.Application.Services.Interceptors
{
    public class AbpApplicationServiceInterceptor : IInterceptor
    {
        private readonly IMethodInvocationValidator _invocationValidator;

        public AbpApplicationServiceInterceptor(IMethodInvocationValidator invocationValidator)
        {
            _invocationValidator = invocationValidator;
        }

        public void Intercept(IInvocation invocation)
        {
            _invocationValidator.Validate(invocation.InvocationTarget, invocation.Method, invocation.Arguments);
            invocation.Proceed();
        }
    }
}
