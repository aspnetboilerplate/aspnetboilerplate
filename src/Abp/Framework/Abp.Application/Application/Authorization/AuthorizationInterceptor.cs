using Castle.DynamicProxy;

namespace Abp.Application.Authorization
{
    /// <summary>
    /// This class is used to intercept methods to make authorization if the method defined <see cref="AbpAuthorizeAttribute"/>.
    /// </summary>
    internal class AuthorizationInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            AuthorizationInterceptionHelper.Authorize<AbpAuthorizeAttribute>(invocation);
            invocation.Proceed();
        }
    }
}
