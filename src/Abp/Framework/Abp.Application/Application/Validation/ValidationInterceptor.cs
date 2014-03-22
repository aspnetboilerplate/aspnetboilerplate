using Castle.DynamicProxy;

namespace Abp.Application.Validation
{
    /// <summary>
    /// This interceptor is used intercept method calls for classes which's methods must be validated.
    /// </summary>
    internal class ValidationInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            new MethodInvocationValidator(
                invocation.Method,
                invocation.Arguments
                ).Validate();

            invocation.Proceed();
        }
    }
}
