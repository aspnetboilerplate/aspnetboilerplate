using Castle.DynamicProxy;

namespace Abp.Application.Services.Dto.Validation
{
    public class ValidationInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var validator = new MethodInvocationValidator(invocation.InvocationTarget, invocation.Method, invocation.Arguments);
            validator.Validate();
            invocation.Proceed();
        }
    }
}
